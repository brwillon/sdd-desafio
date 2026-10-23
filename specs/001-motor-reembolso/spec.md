# Spec — Motor de Cálculo de Reembolso

**Versão:** 1.0 · **Status:** rascunho · **Última alteração:** 2026-10-23

> **Regra de ouro deste arquivo:** ele descreve o QUÊ e o PORQUÊ. Nenhuma linha
> aqui pode citar linguagem, biblioteca, classe, função ou estrutura de pasta.
> Se apareceu solução, o lugar dela é o `plan.md`.
>
> **Teste de aceitação da própria spec:** uma pessoa que nunca viu o projeto
> consegue, lendo só este arquivo, verificar se o sistema está correto?

---

## 1. Problema

O financeiro confere manualmente, item por item, as despesas de cada colaborador
contra a política de reembolso (v3). O processo é lento e produz resultados
divergentes, porque a política admite mais de uma leitura em vários pontos e cada
conferente resolve essas dúvidas por conta própria, sem registro.

## 2. Objetivo

Dado o conjunto de despesas de um colaborador numa competência, o sistema informa,
para cada despesa, o valor reembolsável e a justificativa da decisão, aplicando
sempre as mesmas interpretações da política, todas registradas nesta spec.

## 3. Fora de escopo

- Não detecta automaticamente refeição com cliente ou despesa de representação.
  A descrição da despesa é texto livre e não é interpretada. Isso é orientação para
  o revisor humano (AMB-022).
- Não decide o resultado final de itens em revisão humana. O sistema só os sinaliza
  e informa o motivo (RN-010).
- Não verifica, entre as despesas, se uma duplicata sinalizada é legítima, nem se o
  café da manhã já estava incluído na nota de hospedagem, nem se uma mesma nota de
  hospedagem foi anexada a mais de uma diária. Essa checagem é humana (AMB-007,
  AMB-023, AMB-024).
- Não processa estornos nem devoluções. Eles seguem processo próprio, fora deste
  sistema (AMB-017).
- Não trata de horas extras, plantão, feriado ou fim de semana. Esses dias recebem
  os mesmos limites de qualquer outro dia (AMB-019).
- Não extrai número de diárias, motivo da despesa ou indício de viagem do texto
  livre das despesas (AMB-015, AMB-014).
- Não define comportamento quando `competencia` e `inicio`/`fim` divergem (AMB-011).
- Não efetua pagamento, não converte moedas (todos os valores são em reais) e não
  processa mais de um colaborador por arquivo de entrada.

## 4. Entrada e saída

**Entrada:** conforme `exemplos/despesas-exemplo.json`. Campos e significado:

| Campo | Tipo | Significado | Obrigatório |
|---|---|---|---|
| `colaborador.id` | texto | Identificador do colaborador | Sim |
| `colaborador.nome` | texto | Nome, apenas informativo | Não |
| `colaborador.centro_custo` | texto | Centro de custo, apenas informativo | Não |
| `periodo.competencia` | texto `AAAA-MM` | Mês de competência. É a referência para a regra de período (AMB-011) | Sim |
| `periodo.inicio`, `periodo.fim` | data | Limites informados. Não são usados pelas regras (AMB-011) | Não |
| `despesas` | lista | Despesas do colaborador na competência. A ordem da lista é a "ordem do arquivo" (AMB-002) | Sim |
| `despesas[].id` | texto | Identificador único da despesa | Sim |
| `despesas[].data` | data | Data em que a despesa ocorreu | Sim |
| `despesas[].categoria` | texto | Categoria informada (RN-001) | Sim |
| `despesas[].descricao` | texto | Descrição livre, apenas informativa | Não |
| `despesas[].fornecedor` | texto | Fornecedor da despesa | Sim |
| `despesas[].valor` | número | Valor em reais. Pode ter mais de 2 casas decimais (RN-009) | Sim |
| `despesas[].tem_nota_fiscal` | verdadeiro/falso | Se há nota fiscal | Sim |

**Saída:** um resultado por arquivo de entrada, com um item por despesa.

| Campo | Tipo | Significado |
|---|---|---|
| `colaborador_id` | texto | Copiado da entrada |
| `competencia` | texto | Copiada da entrada |
| `viagem` | verdadeiro/falso | Se a competência foi caracterizada como viagem (RN-006) |
| `total_reembolsavel` | número, 2 casas | Soma dos `valor_reembolsavel` dos itens `aprovado` (RN-011) |
| `resumo` | objeto | Quantidade de itens por status: `aprovado`, `recusado`, `em_revisao` |
| `itens` | lista | Um elemento por despesa da entrada, na mesma ordem |
| `itens[].id` | texto | Id da despesa |
| `itens[].categoria` | texto | Categoria normalizada (RN-001), ou a informada se não reconhecida |
| `itens[].status` | texto | `aprovado` (inclui pagamento parcial), `recusado` (final, valor 0) ou `em_revisao` (valor 0, aguarda revisor humano) |
| `itens[].valor_solicitado` | número, 2 casas | Valor da despesa depois do arredondamento (RN-009) |
| `itens[].valor_reembolsavel` | número, 2 casas | Valor a reembolsar. É 0,00 quando o status é `recusado` ou `em_revisao` |
| `itens[].justificativa` | texto | Explicação em linguagem natural da decisão |
| `itens[].regras` | lista de texto | Ids das regras (RN-xxx) que determinaram o resultado |
| `itens[].alertas` | lista de texto | Advertências para o revisor humano (por exemplo, possível duplicata). Pode ser vazia |

Exemplo de saída para um subconjunto do exemplo oficial (d-001, d-002, d-004 e
d-010, nesta ordem):

```
{
  "colaborador_id": "c-0417",
  "competencia": "2026-07",
  "viagem": true,
  "total_reembolsavel": 340.00,
  "resumo": { "aprovado": 3, "recusado": 1, "em_revisao": 0 },
  "itens": [
    { "id": "d-001", "categoria": "alimentacao", "status": "aprovado",
      "valor_solicitado": 72.50, "valor_reembolsavel": 72.50,
      "justificativa": "Dentro do saldo diário de alimentação (limite 90,00 em viagem).",
      "regras": ["RN-001", "RN-006", "RN-007"], "alertas": [] },
    { "id": "d-002", "categoria": "alimentacao", "status": "aprovado",
      "valor_solicitado": 38.00, "valor_reembolsavel": 17.50,
      "justificativa": "Restava 17,50 do saldo diário de 90,00; excedente cortado.",
      "regras": ["RN-001", "RN-006", "RN-007"], "alertas": [] },
    { "id": "d-004", "categoria": "transporte_urbano", "status": "recusado",
      "valor_solicitado": 100.01, "valor_reembolsavel": 0.00,
      "justificativa": "Valor acima de 100,00 sem nota fiscal.",
      "regras": ["RN-001", "RN-004"], "alertas": [] },
    { "id": "d-010", "categoria": "hospedagem", "status": "aprovado",
      "valor_solicitado": 480.00, "valor_reembolsavel": 250.00,
      "justificativa": "Lançamento conta como uma diária; limite de 250,00; excedente cortado.",
      "regras": ["RN-001", "RN-008"], "alertas": [] }
  ]
}
```

## 5. Regras de negócio

Cada regra recebe um ID (`RN-001`, ...). As tasks vão referenciar esses IDs.
Salvo indicação contrária, os exemplos de aceite usam o arquivo
`exemplos/despesas-exemplo.json` completo. Onde o exemplo diz "sozinho" ou "sem
d-0xx", considera-se o mesmo arquivo com apenas aquelas despesas, mantida a ordem.

### RN-001 — Categorias aceitas e normalização

**Regra:** só são reembolsáveis `alimentacao`, `transporte_urbano` e `hospedagem`.
Antes de comparar, a categoria informada é normalizada: ignora-se caixa, acentos e
espaços nas pontas, e espaço ou hífen entre palavras equivale a sublinhado. Se,
após a normalização, a categoria for igual a uma das três, ela é usada com esse
nome. Qualquer outra categoria torna a despesa `recusado`, com alerta de
encaminhamento para análise humana (o texto do alerta contém "análise humana") e
a categoria informada é mantida na saída. `regras` contém `RN-001` em toda despesa
que passa por este passo, reconhecida ou não.
**Origem:** política do RH, item 9
**Aceite:** d-014 (`ALIMENTACAO`) é tratada como `alimentacao` e fica `aprovado`
com 61,00. d-005 (`coworking`) fica `recusado` com 0,00.

### RN-002 — Valor negativo

**Regra:** despesa com valor negativo é dado inválido e fica `recusado`. Os
demais itens do arquivo são processados normalmente. A justificativa do item é
"Valor negativo é dado inválido." e `regras` contém `RN-002`.
**Origem:** decisão desta spec (a política não trata valores negativos)
**Aceite:** d-009 (−45,00) fica `recusado` com 0,00 e não altera o saldo nem o
total de nenhuma outra despesa.

### RN-003 — Período de competência

**Regra:** a data da despesa deve cair no mês indicado em `competencia`. A despesa
fora do mês fica `em_revisao`, e o revisor humano decide a aprovação ou rejeição. A data cai no mês quando o ano e o
mês da data são iguais aos de `competencia` (AAAA-MM). O primeiro e o último dia do
mês estão dentro. O valor reembolsável do item em revisão é 0,00 e `regras`
contém `RN-003`.
**Origem:** política do RH, item 7
**Aceite:** d-008 (2026-04-15, competência 2026-07) fica `em_revisao` com 0,00.
d-014 (2026-07-31) está dentro do período.

### RN-004 — Nota fiscal obrigatória

**Regra:** despesa com valor **acima** de R$ 100,00 e sem nota fiscal fica
`recusado` com valor 0,00, mesmo que o limite permitisse pagar algo. Até R$ 100,00
inclusive, a nota é dispensada. A verificação usa o valor de cada despesa
individualmente, antes de qualquer corte por limite, e o limiar de R$ 100,00 não
muda em viagem. A regra só avalia itens ainda ativos: item já `recusado` ou
`em_revisao` por passo anterior mantém situação, valor e `regras`, sem receber
`RN-004`. O item recusado por esta regra tem `regras` contendo `RN-004`.
**Origem:** política do RH, item 5
**Aceite:** d-003 (100,00, sem nota) não é recusada por falta de nota. d-004
(100,01, sem nota) fica `recusado`. d-013 (690,00, sem nota) fica `recusado`.

### RN-005 — Duplicatas

**Regra:** duas ou mais despesas que chegam a este passo (ver seção 8) com a mesma
data, a mesma categoria normalizada, o mesmo fornecedor e o mesmo valor formam um
grupo de possíveis duplicatas. Todos os itens do grupo ficam `em_revisao`, com
alerta que cita os ids do grupo.
**Origem:** política do RH, item 8
**Aceite:** d-006 e d-007 ficam `em_revisao`, 0,00 cada, com alerta citando os dois
ids. d-001 e d-002 (fornecedores diferentes) e d-003 e d-004 (valores
diferentes) não formam grupo.

### RN-006 — Viagem

**Regra:** a competência é de viagem se existir ao menos uma despesa de hospedagem
que chegue ao passo de viagem (seção 8) sem ter sido recusada nem enviada à
revisão. Quando é viagem, vale para toda a competência: o limite diário de
alimentação passa de 60,00 para 90,00 e o de transporte urbano de 80,00 para
120,00. O limite de hospedagem (250,00) e o limiar de nota fiscal (100,00) não
mudam.
**Origem:** política do RH, item 6
**Aceite:** no arquivo completo, `viagem` é verdadeiro porque d-010 é aprovada.
Sem d-010 e sem d-013, `viagem` é falso, e d-014 sozinha paga 60,00. Com d-013 como
única hospedagem, `viagem` é falso, pois d-013 é recusada por RN-004.

### RN-007 — Limite diário de alimentação e transporte urbano

**Regra:** o limite vale para a soma da categoria no dia (data da despesa). No
início de cada dia e categoria, o saldo é o limite aplicável (60,00 e 80,00, ou
90,00 e 120,00 em viagem, conforme RN-006). As despesas que chegam a este passo
são processadas na ordem do arquivo. Cada uma recebe o menor valor entre o
solicitado e o saldo restante, e esse valor é abatido do saldo. Cada par (dia,
categoria) tem saldo próprio e independente: alimentação e transporte urbano não
compartilham saldo, e o saldo não passa de um dia para outro. Valor igual ao
limite é pago integralmente. O excedente é cortado. Se o saldo restante for 0,
o item fica `recusado` com 0,00. Fim de semana, feriado e plantão não alteram o
limite. Café da manhã de hotel é alimentação como qualquer outra. Item já `recusado` ou
`em_revisao` em passo anterior não chega a este passo: não consome saldo e mantém
situação, valor e `regras`, sem receber `RN-007`.
**Origem:** política do RH, itens 1, 2 e 4
**Aceite:** no arquivo completo (viagem, limites 90,00 e 120,00):
d-001 paga 72,50 e d-002 paga 17,50, num total de 90,00. d-003 paga 100,00.
d-011 paga 33,33. d-012 paga 47,20. d-014 paga 61,00.
Sem viagem (d-001 e d-002 sozinhas): d-001 paga 60,00 e d-002 fica `recusado`
com 0,00. d-003 sozinha paga 80,00. d-014 sozinha paga 60,00.

### RN-008 — Limite de hospedagem por diária

**Regra:** cada despesa de hospedagem equivale a uma diária, e o reembolsável é o
menor valor entre o solicitado e 250,00. O número de diárias ou noites que
aparecer na descrição é ignorado. Diárias devem ser lançadas uma a uma, e a mesma
nota pode ser anexada a várias.
**Origem:** política do RH, item 3
**Aceite:** d-010 (480,00, "2 diarias") paga 250,00, com 230,00 cortados. d-013
("3 noites") seria limitada a 250,00 por esta regra, mas é recusada antes por
RN-004.

### RN-009 — Arredondamento

**Regra:** todo valor de entrada é arredondado para 2 casas decimais, com
arredondamento bancário (empate vai para o dígito par), logo na leitura, antes de
qualquer regra ou soma.
**Origem:** decisão desta spec (a política não trata centavos)
**Aceite:** d-011 (33,333) tem `valor_solicitado` 33,33. Em casos hipotéticos de
empate, 10,125 vira 10,12 e 10,135 vira 10,14.

### RN-010 — Itens em revisão humana

**Regra:** item `em_revisao` tem `valor_reembolsavel` 0,00, não consome saldo
diário, não conta como evidência de viagem e não entra em `total_reembolsavel`. A
saída informa o motivo e os alertas. A decisão final é do revisor humano, fora do
sistema.
**Origem:** decisão desta spec (política, itens 7 e 8)
**Aceite:** d-006, d-007 e d-008 ficam `em_revisao` com 0,00, não alteram o saldo
das demais despesas de alimentação e não entram no total.

### RN-011 — Totalização e justificativa

**Regra:** `total_reembolsavel` é a soma dos `valor_reembolsavel` de todos os itens
`aprovado`. Cada despesa da entrada aparece uma única vez na saída, na ordem da
entrada, com `status`, `justificativa` e `regras`.
**Origem:** objetivo desta spec
**Aceite:** no arquivo completo, `total_reembolsavel` é 581,53. `resumo` mostra 7
`aprovado`, 4 `recusado` e 3 `em_revisao`, e a saída tem 14 itens.

---

## 6. Ambiguidades identificadas e decisões

> **Esta seção é o coração da spec e vale a maior parte dos 25 pontos do critério 1.**
> Uma ambiguidade que você resolveu no código sem registrar aqui conta como
> não resolvida.

### AMB-001 — Limite "por dia": por dia ou por despesa

**Texto original do RH:** "Alimentação tem limite de R$ 60 por dia." / "Transporte urbano tem limite de R$ 80 por dia."
**O que não está claro:** o teto vale para a soma da categoria no dia, ou para cada despesa individualmente.
**Decisão:** o teto vale para a soma da categoria no dia. Os pares d-001/d-002 e d-006/d-007 são limitados ao teto diário, e o par d-003/d-004 também. Cada par (dia, categoria) tem saldo próprio e independente: alimentação e transporte urbano não compartilham saldo, e o saldo não passa de um dia para outro.
**Justificativa:** a política não fala em limite por refeição, mas em limite diário.
**Regra afetada:** RN-007

### AMB-002 — Como distribuir o corte dentro do dia

**Texto original do RH:** "Despesas acima do limite são reembolsadas parcialmente."
**O que não está claro:** quando várias despesas excedem o teto do dia, qual delas sofre o corte: a primeira, a última, ou um rateio.
**Decisão:** vale a ordem do arquivo. O primeiro item consome o saldo e os seguintes recebem o que restar.
**Justificativa:** o limite diário é um saldo a consumir, que é aproveitado pelo lançamento seguinte.
**Regra afetada:** RN-007

### AMB-003 — "Reembolsadas parcialmente"

**Texto original do RH:** "Despesas acima do limite são reembolsadas parcialmente."
**O que não está claro:** paga-se o limite e corta-se o excedente, ou recusa-se o item inteiro.
**Decisão:** paga o limite e corta o excedente.
**Justificativa:** o valor do limite representa o valor a que o colaborador tem direito.
**Regra afetada:** RN-007, RN-008

### AMB-004 — Hospedagem "por diária"

**Texto original do RH:** "Hospedagem tem limite de R$ 250 por diária."
**O que não está claro:** o limite vale por diária (com o número de diárias do lançamento) ou por lançamento.
**Decisão:** O limite de 250,00 vale por lançamento, e cada lançamento de hospedagem equivale a uma diária (AMB-015). Quantidades de noites ou diárias na descrição são ignoradas. Assim, d-010 (480,00) paga 250,00.
**Justificativa:** A descrição é texto livre e não confiável; o lançamento individual por diária (AMB-015, AMB-024) evita inferir a quantidade.
**Regra afetada:** RN-008

### AMB-005 — Nota fiscal: a qual valor se aplica

**Texto original do RH:** "Nota fiscal é obrigatória acima de R$ 100."
**O que não está claro:** o limite vale para o valor de cada despesa, para a soma do dia, ou para o valor depois do corte do limite.
**Decisão:** vale para o valor de cada despesa individualmente.
**Justificativa:** despesas acima de R$ 100 exigem evidência para aprovação, mesmo que o limite de diária seja menor que esse valor.
**Regra afetada:** RN-004

### AMB-006 — Quais limites a viagem amplia

**Texto original do RH:** "Colaborador em viagem tem limites ampliados em 50%."
**O que não está claro:** a ampliação vale para os três tetos diários, só para alimentação e transporte, ou também para o limiar de R$ 100 da nota fiscal.
**Decisão:** só alimentação e transporte urbano. A hospedagem e o limiar da nota ficam como estão.
**Justificativa:** a hospedagem é a evidência da ampliação e não muda o seu próprio limite nem a exigência de nota, que continua em R$ 100.
**Regra afetada:** RN-006

### AMB-007 — O que é duplicata

**Texto original do RH:** "Duplicatas devem ser tratadas."
**O que não está claro:** quais campos precisam coincidir para duas despesas serem duplicatas.
**Decisão:** data, categoria, fornecedor e valor. A duplicata é uma advertência, e as informações são checadas depois, em ação de validação fora do escopo.
**Justificativa:** pode haver mais de um consumo igual, no mesmo dia, no mesmo fornecedor e categoria.
**Regra afetada:** RN-005

### AMB-008 — Como tratar as duplicatas

**Texto original do RH:** "Duplicatas devem ser tratadas."
**O que não está claro:** o tratamento pode ser descartar a segunda, recusar as duas, pagar as duas com sinalização, ou revisão humana.
**Decisão:** todas as duplicatas, ou seja, todos os itens do grupo, vão para revisão humana, com valor 0,00 até a decisão.
**Justificativa:** todos os itens identificados como duplicata devem ser checados.
**Regra afetada:** RN-005, RN-010

### AMB-009 — Nota fiscal com exatamente R$ 100,00

**Texto original do RH:** "Nota fiscal é obrigatória acima de R$ 100."
**O que não está claro:** a despesa de exatamente R$ 100,00 precisa de nota ou não.
**Decisão:** "acima" é estrito. d-003 (100,00) não precisa de nota, e d-004 (100,01) precisa.
**Justificativa:** até R$ 100 se entende que é tolerado sem nota.
**Regra afetada:** RN-004

### AMB-010 — Valor exatamente igual ao limite

**Texto original do RH:** "Despesas acima do limite são reembolsadas parcialmente."
**O que não está claro:** o valor igual ao limite é pago integralmente ou já conta como excesso.
**Decisão:** igual ao limite está dentro e é pago integralmente.
**Justificativa:** o pagamento parcial só vale acima do limite, conforme o saldo ainda existente na categoria.
**Regra afetada:** RN-007, RN-008

### AMB-011 — Extremos do período e fonte da verdade

**Texto original do RH:** "Despesas devem ser lançadas dentro do período de competência."
**O que não está claro:** a referência do período são os campos `inicio`/`fim` (inclusivos ou exclusivos) ou o campo `competencia`.
**Decisão:** a fonte é o campo `competencia` (por exemplo, "2026-07") e não `inicio`/`fim`. Se os dois divergirem, não há regra definida (seção 10). A comparação é por ano e mês.
**Justificativa:** o reembolso está atrelado à competência informada.
**Regra afetada:** RN-003

### AMB-012 — Arredondamento

**Texto original do RH:** a política não diz nada sobre centavos.
**O que não está claro:** como tratar valores com mais de 2 casas decimais (d-011 = 33,333): arredondar para cima, truncar, arredondamento bancário ou rejeitar.
**Decisão:** arredondamento bancário, com 2 casas decimais, na leitura de cada valor.
**Justificativa:** segue o modelo bancário e evita divergência entre saldos.
**Regra afetada:** RN-009

### AMB-013 — Rótulo da categoria em maiúsculas

**Texto original do RH:** "Categorias fora da política não são reembolsáveis."
**O que não está claro:** `ALIMENTACAO` (d-014) é alimentação, está fora da política ou é erro de dado.
**Decisão:** a comparação ignora maiúsculas e minúsculas, então d-014 é alimentação e entra no limite. Variações de acento e espaço também são normalizadas quando for possível inferir uma categoria aceita. Os demais casos são rejeitados e encaminhados para análise humana.
**Justificativa:** categorias não identificadas devem ser rejeitadas e encaminhadas à análise humana.
**Regra afetada:** RN-001

### AMB-014 — O que é "em viagem"

**Texto original do RH:** "Colaborador em viagem tem limites ampliados em 50%."
**O que não está claro:** a entrada não tem campo de viagem. A viagem pode ser inferida pelo dia, pelo período ou por toda a competência.
**Decisão:** toda a competência é viagem se houver qualquer hospedagem aprovada. Uma hospedagem recusada ou em revisão não conta.
**Justificativa:** sem hospedagem trata-se de deslocamento curto, que não caracteriza viagem.
**Regra afetada:** RN-006

### AMB-015 — Número de diárias só no texto livre

**Texto original do RH:** "Hospedagem tem limite de R$ 250 por diária."
**O que não está claro:** o número de diárias só existe na descrição ("2 diarias", "3 noites"). Pode ser extraído do texto, assumido como 1 ou calculado.
**Decisão:** cada diária deve ser lançada individualmente. Se a descrição de um lançamento indicar mais de uma, ele conta como uma única diária.
**Justificativa:** a quantidade escrita no texto não é confiável, então o lançamento individual evita depender dela.
**Regra afetada:** RN-008

### AMB-016 — Data da despesa × data do lançamento

**Texto original do RH:** "Despesas devem ser lançadas dentro do período de competência."
**O que não está claro:** a entrada só tem a data da despesa, sem data de lançamento. Uma despesa antiga lançada com atraso (d-008) pode ser recusada, aceita ou depender de exceção.
**Decisão:** a despesa fora do período é aceita com exceção: vai para revisão humana (a comparação usa a data da despesa).
**Justificativa:** a aprovação ou rejeição fica a cargo do revisor humano.
**Regra afetada:** RN-003, RN-010

### AMB-017 — Estorno

**Texto original do RH:** a política não trata valores negativos.
**O que não está claro:** o estorno (d-009, −45,00) pode ser ignorado, abatido do total, abatido do dia ou rejeitado.
**Decisão:** é dado inválido e é rejeitado (somente o item).
**Justificativa:** o processo é de reembolso e recebe apenas valores reembolsáveis. Devoluções são lançadas em processo específico, fora deste escopo.
**Regra afetada:** RN-002

### AMB-018 — Categoria sem mapeamento

**Texto original do RH:** "Categorias fora da política não são reembolsáveis."
**O que não está claro:** a política só nomeia três categorias. `coworking` (d-005) poderia ser tratada como hospedagem ou como categoria desconhecida.
**Decisão:** a lista fechada é alimentação, transporte urbano e hospedagem. Coworking está fora e vale 0,00.
**Justificativa:** somente categorias reembolsáveis são válidas, e as demais são rejeitadas.
**Regra afetada:** RN-001

### AMB-019 — Fim de semana e plantão

**Texto original do RH:** "Alimentação tem limite de R$ 60 por dia."
**O que não está claro:** "dia" poderia significar dia útil, ou fim de semana e plantão (d-012, sábado) poderiam ter regra própria.
**Decisão:** é um dia como qualquer outro, e vale o limite normal de acordo com as demais regras.
**Justificativa:** fins de semana, feriados e plantões são tratados externamente como horas extras trabalhadas, sem mudar os limites de reembolso.
**Regra afetada:** RN-007

### AMB-021 — Precedência: nota fiscal × limite

**Texto original do RH:** "Despesas acima do limite são reembolsadas parcialmente." / "Nota fiscal é obrigatória acima de R$ 100."
**O que não está claro:** a despesa sem nota deve ser recusada por inteiro ou paga até o valor que dispensaria a nota.
**Decisão:** sem nota, recusa total (zera o item), mesmo que o limite permitisse pagar algo.
**Justificativa:** a nota é obrigatória para aceitação.
**Regra afetada:** RN-004, seção 8

### AMB-022 — Despesa com terceiros

**Texto original do RH:** "Categorias fora da política não são reembolsáveis."
**O que não está claro:** refeição com cliente (d-001, "Almoco com cliente") poderia ser representação, uma categoria fora da política, ou dividir o limite por pessoa.
**Decisão:** não há detecção automática. A descrição é texto livre e não é interpretada; a decisão vira orientação para a revisão humana. d-001 fica `aprovado` e entra no limite diário normalmente.
**Justificativa:** o reembolso é exclusivo do colaborador, mas a entrada não traz dado confiável para identificar o terceiro.
**Regra afetada:** nenhuma (ver seção 3)

### AMB-023 — Sobreposição entre categorias na mesma estadia

**Texto original do RH:** "Alimentação tem limite de R$ 60 por dia." / "Hospedagem tem limite de R$ 250 por diária."
**O que não está claro:** o café da manhã do hotel (d-011) pode estar incluído na diária (d-010).
**Decisão:** são despesas independentes, e o café entra no limite diário de alimentação, desde que não faça parte da nota de hospedagem. Essa checagem é feita na validação humana, fora do escopo.
**Justificativa:** o café pode não estar incluído na diária de hospedagem.
**Regra afetada:** RN-007

### AMB-024 — Despesa de vários dias, com data única

**Texto original do RH:** "Hospedagem tem limite de R$ 250 por diária."
**O que não está claro:** uma hospedagem de várias noites tem uma data só, o que afeta a atribuição de dias.
**Decisão:** cada diária deve ser incluída de forma independente, e a mesma nota pode ser anexada a todas. A checagem fica com a validação humana.
**Justificativa:** padroniza e facilita o processo.
**Regra afetada:** RN-008

---

## 7. Casos de borda

| Caso | Entrada | Comportamento esperado | Regra |
|---|---|---|---|
| Nota no limiar exato | d-003 (100,00, sem nota) | Não exige nota. `aprovado` com 100,00 | RN-004 |
| Um centavo acima do limiar | d-004 (100,01, sem nota) | `recusado`, 0,00 | RN-004 |
| Sem nota, mesmo dentro do limite | d-013 (690,00, sem nota) | `recusado`, 0,00 | RN-004 |
| Duplicata | d-006 e d-007 | Ambas `em_revisao`, 0,00, alerta com os dois ids | RN-005 |
| Fora do período | d-008 (2026-04-15) | `em_revisao`, 0,00 | RN-003 |
| Último dia do mês | d-014 (2026-07-31) | Dentro do período | RN-003 |
| Estorno | d-009 (−45,00) | `recusado`, 0,00. Demais itens intactos | RN-002 |
| Categoria fora da política | d-005 (coworking) | `recusado`, 0,00 | RN-001 |
| Categoria em maiúsculas | d-014 (`ALIMENTACAO`, 61,00) | Normalizada. Paga 61,00 em viagem e 60,00 sem viagem | RN-001, RN-007 |
| Mais de 2 casas decimais | d-011 (33,333) | `valor_solicitado` 33,33 | RN-009 |
| Diária múltipla no texto | d-010 (480,00, "2 diarias") | Conta como 1 diária, paga 250,00 | RN-008 |
| Dois itens no mesmo dia | d-001 e d-002 | Em viagem: 72,50 e 17,50. Sem viagem: 60,00 e 0,00 (`recusado`) | RN-007 |
| Dia de fim de semana | d-012 (sábado, 47,20) | Sem tratamento especial. `aprovado` com 47,20 | RN-007 |
| Refeição com cliente | d-001 | Sem detecção. `aprovado` normalmente | seção 3 |
| Valor igual ao limite (hipotético) | Alimentação 60,00 sem viagem | Paga 60,00 integralmente | RN-007 |

## 8. Ordem de aplicação das regras

A ordem muda o resultado: um item recusado ou em revisão não consome saldo, não
participa da comparação de duplicatas e não conta como evidência de viagem.

1. **Arredondamento** do valor (RN-009).
2. **Valor negativo** (RN-002): `recusado`.
3. **Categoria** (RN-001): normaliza e, se não reconhecida, `recusado`.
4. **Período** (RN-003): fora da competência, `em_revisao`.
5. **Nota fiscal** (RN-004): acima de 100,00 sem nota, `recusado`.
6. **Duplicata** (RN-005): grupos entre os itens ainda ativos, `em_revisao`.
7. **Viagem** (RN-006): decidida com as hospedagens ainda ativas.
8. **Limite** (RN-007 e RN-008): alimentação e transporte por dia, em ordem do
   arquivo; hospedagem por lançamento.
9. **Totalização** (RN-011).

Consequências: duas despesas iguais e sem nota acima de 100,00 são ambas
`recusado` (passo 5), e não duplicatas (passo 6). A nota (passo 5) vale antes do
limite (passo 8), o que atende à AMB-021.

## 9. Critérios de aceite

O sistema está pronto quando:

- [ ] Para o arquivo `exemplos/despesas-exemplo.json`, `total_reembolsavel` é 581,53 e `viagem` é verdadeiro.
- [ ] No mesmo arquivo, os itens aprovados são d-001 (72,50), d-002 (17,50), d-003 (100,00), d-010 (250,00), d-011 (33,33), d-012 (47,20) e d-014 (61,00).
- [ ] No mesmo arquivo, d-004, d-005, d-009 e d-013 são `recusado`, e d-006, d-007 e d-008 são `em_revisao`, todos com 0,00.
- [ ] `resumo` indica 7 `aprovado`, 4 `recusado` e 3 `em_revisao`, e a lista de itens tem 14 elementos, na ordem da entrada.
- [ ] d-001 e d-002 sozinhas pagam 60,00 e 0,00; d-003 sozinha paga 80,00; d-014 sozinha paga 60,00.
- [ ] Sem d-010 e d-013, `viagem` é falso.
- [ ] Todo item tem justificativa e lista de regras que citam RN existentes nesta spec.
- [ ] Todo item `em_revisao` ou `recusado` tem `valor_reembolsavel` 0,00.
- [ ] Executar duas vezes com a mesma entrada produz a mesma saída.

## 10. O que fica em aberto

- **AMB-011:** quando `competencia` e `inicio`/`fim` divergem, não há regra definida.
- **AMB-020:** o número não é usado na numeração das ambiguidades. Não há decisão registrada.
- **Categoria irreconhecível × categoria fora da política (AMB-013 × AMB-018):** não há critério que diferencie os dois casos. Decisão provisória: ambos ficam `recusado`, com alerta de encaminhamento para análise humana.
- **Saldo diário esgotado:** o item que recebe 0,00 por saldo esgotado fica `recusado` (provisório).
- **Fornecedor na duplicata:** a comparação é por igualdade literal do texto (provisório).
- **Campos obrigatórios ausentes, arquivo inválido ou valor igual a zero:** sem regra definida.
