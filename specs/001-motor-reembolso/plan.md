# Plano Técnico — Motor de Cálculo de Reembolso

**Versão:** 1.0 · **Baseado na spec:** 1.0

> Aqui mora o COMO. Este arquivo pode e deve falar de linguagem, biblioteca e
> arquitetura. O que ele **não** pode é introduzir regra de negócio nova — se
> apareceu uma, ela pertence à `spec.md`.

---

## 1. Stack

| Escolha | O quê | Por quê | O que descartei e por quê |
|---|---|---|---|
| Linguagem | C# 13 / .NET 9 | `decimal` nativo, tipos `record` imutáveis, nullable e `DateOnly`; CLI de um binário só | — |
| Testes | xUnit | Um `[Fact]`/`[Theory]` por requisito, nome do método carrega o id da RN | NUnit e MSTest: sem ganho técnico para este escopo, a escolha é só preferência |
| Parsing/validação | `System.Text.Json` (BCL) | Já vem no .NET, lê número direto como `decimal`, permite controlar a ordem das propriedades na saída (DT-004) | Newtonsoft.Json: dependência externa desnecessária |
| Aritmética monetária | `decimal` | Base 10 exata; `Math.Round(v, 2, MidpointRounding.ToEven)` implementa o arredondamento bancário da RN-009 | `double`/`float`: erro de ponto flutuante em dinheiro (ex.: 0,1 + 0,2 ≠ 0,3), que quebraria saldos e o total (DT-001) |

## 2. Arquitetura

```
entrada JSON ──► Reembolso.Cli (lê arquivo, desserializa)
                      │  DespesasDto
                      ▼
              Reembolso.Core: Pipeline de passos puros
   1 Arredondamento → 2 ValorNegativo → 3 Categoria → 4 Periodo → 5 NotaFiscal
   → 6 Duplicata → 7 Viagem → 8 Limite → 9 Totalizacao
                      │  Resultado
                      ▼
              Reembolso.Cli (serializa, grava arquivo) ──► saída JSON
```

Um tipo por passo da seção 8 da spec, executados nessa ordem. Cada passo recebe o
contexto de avaliação e devolve um novo contexto; nenhum passo faz I/O nem guarda
estado.

**Fronteiras:**

- `src/Reembolso.Core` — regras puras: sem arquivo, sem console, sem relógio, sem
  JSON. Entra um modelo em memória, sai um modelo em memória.
- `src/Reembolso.Cli` — único projeto com I/O: argumentos (`calcular --input
  --output`), leitura/escrita de arquivo, mapeamento JSON ⇄ modelo e código de
  saída.
- `tests/Reembolso.Tests` — xUnit, referencia os dois projetos.

Mudança de política afeta só `Core`; mudança de formato de arquivo afeta só `Cli`.

## 3. Modelo de dados

Todos `record` imutáveis, em `Reembolso.Core`:

- **`Despesa`** — espelha a entrada (seção 4 da spec): `Id`, `Data` (`DateOnly`),
  `Categoria` (texto original), `Fornecedor`, `Valor` (`decimal`), `TemNotaFiscal`,
  `Descricao?`.
- **`Competencia`** — ano e mês, comparados por igualdade com os da `Data`.
- **`ItemAvaliado`** — a despesa mais o estado da avaliação: `Categoria`
  normalizada, `ValorSolicitado` (já arredondado), `Situacao`, `ValorReembolsavel`,
  `Justificativa`, `Regras` (lista de ids `RN-xxx`) e `Alertas`.
- **`Situacao`** — `Ativo` (ainda em avaliação; estado interno do pipeline, não
  aparece na saída), `Aprovado`, `Recusado`, `EmRevisao`. Só `Ativo` participa dos
  passos seguintes (consequência direta da seção 8 da spec). Ao fim do passo 8,
  todo item `Ativo` vira `Aprovado`.
- **`ContextoAvaliacao`** — `Competencia`, `Viagem` (preenchido no passo 7) e a
  lista de `ItemAvaliado` na ordem do arquivo.
- **`Resultado`** — `ColaboradorId`, `Competencia`, `Viagem`, `TotalReembolsavel`,
  `Resumo` e os itens, no formato da seção 4 da spec.

A justificativa e as regras são acumuladas pelo próprio passo que decide, de modo
que cada item sai explicando quem o determinou.

## 4. Como a política é representada

Um `record PoliticaReembolso` imutável com os limites já definidos na spec:

- `LimiteAlimentacaoDia`, `LimiteTransporteDia` (valores normais);
- `LimiteAlimentacaoDiaViagem`, `LimiteTransporteDiaViagem` (valores em viagem);
- `LimiteHospedagemDiaria`;
- `LimiarNotaFiscal`.

Existe `PoliticaReembolso.Padrao` com os valores da spec (60, 80, 90, 120, 250 e
100). Cada passo que precisa de um limite recebe a política pelo construtor; nenhum
passo lê constante global. Os testes podem montar uma política própria com `with`.

Os valores não vão para arquivo de configuração: a política é versionada (v3) e
muda junto com a spec, então o lugar dela é o código, revisado por commit. Se um dia
precisar ser externa, só a criação da instância muda — os passos não.

## 5. Decisões técnicas

### DT-001 — `decimal` para valores monetários

**Contexto:** a spec exige somas, saldos e total exatos ao centavo, com
arredondamento bancário (RN-009, RN-007, RN-011).
**Decisão:** todo valor monetário é `decimal`; arredondamento com
`Math.Round(v, 2, MidpointRounding.ToEven)`; leitura do JSON com `GetDecimal()`.
**Alternativa descartada:** `double` — representação binária inexata; 33,333 ou
somas de saldos poderiam divergir em centavos e o teste de empate (10,125 → 10,12)
ficaria sujeito a erro de representação.
**Consequência:** cálculos exatos e testáveis por igualdade; o custo é desempenho
marginal, irrelevante para um colaborador por arquivo.

### DT-002 — Pipeline de passos em vez de `if` aninhado

**Contexto:** a ordem de aplicação (seção 8) muda o resultado: item recusado ou em
revisão não consome saldo, não entra na comparação de duplicatas e não conta como
viagem.
**Decisão:** uma interface de passo (`Contexto Aplicar(Contexto)`) com um tipo por
passo da seção 8, compostos numa lista ordenada. Cada passo só olha itens `Ativo`.
**Alternativa descartada:** um método único com `if` aninhados por despesa — mistura
regras que são de lista (duplicata, viagem, saldo diário) com regras de item, e a
precedência fica implícita na indentação.
**Consequência:** a precedência é a própria lista; cada passo é testável isolado e
mapeia para uma RN. Mudar a ordem é mudar uma linha, mas exige alterar antes a
seção 8 da spec.

### DT-003 — Política em `record` imutável injetado

**Contexto:** os limites mudam se a política mudar, e os testes precisam variá-los
sem tocar os passos.
**Decisão:** `PoliticaReembolso` como `record` com instância `Padrao`, passada
pelo construtor aos passos que a usam (seção 4 deste plano).
**Alternativa descartada:** constantes estáticas espalhadas nos passos — o valor
fica duplicado e não é possível testar com outro limite.
**Consequência:** um único ponto de mudança; o custo é um parâmetro de construtor a
mais.

### DT-004 — `System.Text.Json` com ordem de propriedades fixa

**Contexto:** a seção 9 da spec exige que duas execuções com a mesma entrada
produzam a mesma saída, e a seção 4 define a ordem dos campos e números com 2 casas.
**Decisão:** DTOs de saída próprios, com `[JsonPropertyOrder]` explícito em todos os
campos, nomes em `snake_case` conforme a spec, e um `JsonConverter<decimal>` que
escreve sempre 2 casas (`72.50`, `340.00`, `0.00`). A saída não depende de
dicionários, de cultura (usa `InvariantCulture`) nem de data/hora atual.
**Alternativa descartada:** serializar o modelo de domínio direto — a ordem e o
formato passariam a depender de detalhes internos, e `decimal` de escala 0 sairia
como `100` em vez de `100.00`.
**Consequência:** saída byte a byte reproduzível, testável por comparação de texto;
o custo é uma camada de DTOs na `Cli`.

## 6. Estratégia de testes

- **Nível:** predominantemente unitário (cada passo do pipeline isolado, no
  `Core`); poucos testes de composição do pipeline; um único teste ponta a ponta
  com `exemplos/despesas-exemplo.json` passando pela `Cli`.
- **Cada `RN-NNN` da spec tem teste?** Sim: ao menos um teste por RN (RN-001 a
  RN-011). A tabela de cobertura do `tasks.md` liga RN → task → teste, e um teste
  sem RN, ou RN sem teste, é lacuna visível ali.
- **Casos de borda da seção 7 da spec:** um teste por linha da tabela (14 linhas).
- **Critérios de aceite da seção 9:** um teste por cenário mínimo (total 581,53 e
  `viagem` verdadeiro; itens aprovados; recusados e em revisão com 0,00; resumo e
  14 itens na ordem; d-001/d-002, d-003 e d-014 sozinhas; sem d-010 e d-013 a
  `viagem` é falsa; justificativa e regras existentes em todo item; execução
  repetida idêntica).
- **Nomenclatura:** `RNxxx_Cenario_Resultado` (ex.:
  `RN004_ValorExatamente100SemNota_NaoRecusa`) e `AMBxxx_...` quando o teste
  exercita uma decisão da seção 6. O prefixo fecha a rastreabilidade com a spec.

## 7. Riscos

| Risco | Probabilidade | O que faço se acontecer |
|---|---|---|
| `decimal` sair no JSON sem 2 casas (`100` em vez de `100.00`) ou com cultura errada | Média | O conversor da DT-004 formata sempre com 2 casas e `InvariantCulture`; o teste ponta a ponta compara o texto da saída. Se ainda falhar, escrever o número bruto com `Utf8JsonWriter.WriteRawValue` |
| Passos acoplados por estado (viagem e saldo dependem do que os passos anteriores deixaram `Ativo`) levando a resultado errado quando a ordem é alterada | Média | Cada passo declara em teste o que lê e o que altera; teste de precedência (seção 8 da spec: duas despesas iguais sem nota > 100,00 são `recusado`, não duplicatas). Se a ordem precisar mudar, corrigir antes a spec |
| Normalização de categoria divergente da RN-001 (acentos, hífen, espaço) | Média | Normalizar por decomposição Unicode (FormD) removendo marcas diacríticas e, depois, aplicar caixa e troca de separadores; um teste por variação. Se surgir um caso não coberto pela spec, registrar em `DECISIONS.md` antes de codificar |
| Entrada inválida (campo obrigatório ausente, arquivo malformado, valor 0): a spec diz que não há regra definida (seção 10) | Alta | Não inventar regra no código. A `Cli` falha com mensagem técnica e código de saída diferente de zero somente para JSON ilegível; os demais casos ficam sem tratamento específico até a spec decidir. Registrar a pendência e levar ao dono da spec |
| `inicio`/`fim` divergindo de `competencia` (AMB-011 em aberto) | Baixa | Usar só `competencia`, conforme a spec; ignorar `inicio`/`fim`. Se o RH exigir outro comportamento, é mudança de spec |
| Mudança de requisito no Dia 2 atingir mais de um passo | Média | Política injetada (DT-003) e um tipo por passo (DT-002) limitam a mudança; novas tasks continuam a numeração, sem renumerar as antigas |
