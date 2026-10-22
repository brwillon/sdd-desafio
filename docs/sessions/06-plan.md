 ▐▛███▛█   Claude Code v2.1.308
▝▜██████▀  Sonnet 5.5 · Claude Pro
 ▝▝   ▝▝   D:\github\sdd-desafio


❯ /clear                                                                        

❯ Preencha specs/001-motor-reembolso/plan.md. Não introduza regra de negócio    
  nova; se achar que falta uma, PARE e me avise.                                
  Stack: C# / .NET 9, xUnit, System.Text.Json, decimal. Descartados: double     
  (erro de ponto flutuante em dinheiro), Newtonsoft.Json (dependência           
  desnecessária), NUnit/MSTest (preferência).                                   
  Arquitetura: Reembolso.Core com pipeline de passos puros na ordem de          
  precedência da seção 8 da spec; Reembolso.Cli como casca de I/O.              
  Política: record imutável com os limites, uma instância padrão injetada nos   
  passos.                                                                       
  DTs: DT-001 decimal vs double; DT-002 pipeline de passos vs if-aninhado;      
  DT-003 política em record injetado; DT-004 System.Text.Json com ordem de      
  propriedades fixa para a saída determinística (seção 9 da spec).              
  Estratégia de testes: um teste por RN, um por linha da tabela de bordas       
  (seção 7), um por cenário mínimo da seção 9, e um ponta a ponta com o arquivo 
  de exemplo. Nomenclatura RNxxx_Cenario_Resultado.                             
  Riscos: pelo menos 3, com o que fazer se acontecer.                           

● Vou ler a spec e o plan atual antes de preencher.

  Searched for 1 pattern, read 3 files (ctrl+o to expand)

● Write(specs\001-motor-reembolso\plan.md)
  ⎿  Added 130 lines, removed 30 lines
       12  
       13  | Escolha | O quê | Por quê | O que descartei e por quê |
       14  |---|---|---|---|
       15 -| Linguagem | | | |                                           
       16 -| Testes | | | |                                              
       17 -| Parsing/validação | | | |                                   
       18 -| Aritmética monetária | | | |                                
       15 +| Linguagem | C# 13 / .NET 9 | `decimal` nativo, tipos `record
          +` imutáveis, nullable e `DateOnly`; CLI de um binário só | — |
       16 +| Testes | xUnit | Um `[Fact]`/`[Theory]` por requisito, nome 
          +do método carrega o id da RN | NUnit e MSTest: sem ganho técni
          +co para este escopo, a escolha é só preferência |             
       17 +| Parsing/validação | `System.Text.Json` (BCL) | Já vem no .NE
          +T, lê número direto como `decimal`, permite controlar a ordem 
          +das propriedades na saída (DT-004) | Newtonsoft.Json: dependên
          +cia externa desnecessária |                                   
       18 +| Aritmética monetária | `decimal` | Base 10 exata; `Math.Roun
          +d(v, 2, MidpointRounding.ToEven)` implementa o arredondamento 
          +bancário da RN-009 | `double`/`float`: erro de ponto flutuante
          + em dinheiro (ex.: 0,1 + 0,2 ≠ 0,3), que quebraria saldos e o 
          +total (DT-001) |                                              
       19  
       20 -<A linha de aritmética monetária não é decoração. Ponto flutua
          -nte em dinheiro é                                             
       21 -a fonte de bug mais previsível deste projeto.>                
       22 -                                                              
       20  ## 2. Arquitetura
       21  
       25 -<Diagrama em blocos ou lista. Quais são as peças, o que cada u
          -ma faz, como                                                  
       26 -conversam. Uma tela, não uma tese.>                           
       27 -                                                              
       22  ```
       29 -entrada JSON → <...> → <...> → saída JSON                     
       23 +entrada JSON ──► Reembolso.Cli (lê arquivo, desserializa)     
       24 +                      │  DespesasDto                          
       25 +                      ▼                                       
       26 +              Reembolso.Core: Pipeline de passos puros        
       27 +   1 Arredondamento → 2 ValorNegativo → 3 Categoria → 4 Period
          +o → 5 NotaFiscal                                              
       28 +   → 6 Duplicata → 7 Viagem → 8 Limite → 9 Totalizacao        
       29 +                      │  Resultado                            
       30 +                      ▼                                       
       31 +              Reembolso.Cli (serializa, grava arquivo) ──► saí
          +da JSON                                                       
       32  ```
       33  
       32 -**Fronteiras:** <o que é núcleo de regra de negócio puro e o q
          -ue é I/O. Onde                                                
       33 -essa linha está desenhada determina o quanto o sistema vai res
          -istir a mudança                                               
       34 -de requisito.>                                                
       34 +Um tipo por passo da seção 8 da spec, executados nessa ordem. 
          +Cada passo recebe o                                           
       35 +contexto de avaliação e devolve um novo contexto; nenhum passo
          + faz I/O nem guarda                                           
       36 +estado.                                                       
       37  
       38 +**Fronteiras:**                                               
       39 +                                                              
       40 +- `src/Reembolso.Core` — regras puras: sem arquivo, sem consol
          +e, sem relógio, sem                                           
       41 +  JSON. Entra um modelo em memória, sai um modelo em memória. 
       42 +- `src/Reembolso.Cli` — único projeto com I/O: argumentos (`ca
          +lcular --input                                                
       43 +  --output`), leitura/escrita de arquivo, mapeamento JSON ⇄ mo
          +delo e código de                                              
       44 +  saída.                                                      
       45 +- `tests/Reembolso.Tests` — xUnit, referencia os dois projetos
          +.                                                             
       46 +                                                              
       47 +Mudança de política afeta só `Core`; mudança de formato de arq
          +uivo afeta só `Cli`.                                          
       48 +                                                              
       49  ## 3. Modelo de dados
       50  
       38 -<Estruturas internas. Como uma despesa é representada, como um
          - resultado de                                                 
       39 -avaliação é representado, o que carrega a justificativa.>     
       51 +Todos `record` imutáveis, em `Reembolso.Core`:                
       52  
       53 +- **`Despesa`** — espelha a entrada (seção 4 da spec): `Id`, `
          +Data` (`DateOnly`),                                           
       54 +  `Categoria` (texto original), `Fornecedor`, `Valor` (`decima
          +l`), `TemNotaFiscal`,                                         
       55 +  `Descricao?`.                                               
       56 +- **`Competencia`** — ano e mês, comparados por igualdade com 
          +os da `Data`.                                                 
       57 +- **`ItemAvaliado`** — a despesa mais o estado da avaliação: `
          +Categoria`                                                    
       58 +  normalizada, `ValorSolicitado` (já arredondado), `Situacao`,
          + `ValorReembolsavel`,                                         
       59 +  `Justificativa`, `Regras` (lista de ids `RN-xxx`) e `Alertas
          +`.                                                            
       60 +- **`Situacao`** — `Ativo` (ainda em avaliação; estado interno
          + do pipeline, não                                             
       61 +  aparece na saída), `Aprovado`, `Recusado`, `EmRevisao`. Só `
          +Ativo` participa dos                                          
       62 +  passos seguintes (consequência direta da seção 8 da spec). A
          +o fim do passo 8,                                             
       63 +  todo item `Ativo` vira `Aprovado`.                          
       64 +- **`ContextoAvaliacao`** — `Competencia`, `Viagem` (preenchid
          +o no passo 7) e a                                             
       65 +  lista de `ItemAvaliado` na ordem do arquivo.                
       66 +- **`Resultado`** — `ColaboradorId`, `Competencia`, `Viagem`, 
          +`TotalReembolsavel`,                                          
       67 +  `Resumo` e os itens, no formato da seção 4 da spec.         
       68 +                                                              
       69 +A justificativa e as regras são acumuladas pelo próprio passo 
          +que decide, de modo                                           
       70 +que cada item sai explicando quem o determinou.               
       71 +                                                              
       72  ## 4. Como a política é representada
       73  
       43 -<Os limites vivem onde? Constantes no código, arquivo de confi
          -guração, tabela?                                              
       44 -Esta decisão é a que mais barato ou mais caro vai custar se a 
          -política mudar.>                                              
       74 +Um `record PoliticaReembolso` imutável com os limites já defin
          +idos na spec:                                                 
       75  
       76 +- `LimiteAlimentacaoDia`, `LimiteTransporteDia` (valores norma
          +is);                                                          
       77 +- `LimiteAlimentacaoDiaViagem`, `LimiteTransporteDiaViagem` (v
          +alores em viagem);                                            
       78 +- `LimiteHospedagemDiaria`;                                   
       79 +- `LimiarNotaFiscal`.                                         
       80 +                                                              
       81 +Existe `PoliticaReembolso.Padrao` com os valores da spec (60, 
          +80, 90, 120, 250 e                                            
       82 +100). Cada passo que precisa de um limite recebe a política pe
          +lo construtor; nenhum                                         
       83 +passo lê constante global. Os testes podem montar uma política
          + própria com `with`.                                          
       84 +                                                              
       85 +Os valores não vão para arquivo de configuração: a política é 
          +versionada (v3) e                                             
       86 +muda junto com a spec, então o lugar dela é o código, revisado
          + por commit. Se um dia                                        
       87 +precisar ser externa, só a criação da instância muda — os pass
          +os não.                                                       
       88 +                                                              
       89  ## 5. Decisões técnicas
       90  
       48 -### DT-001 — <decisão>                                        
       91 +### DT-001 — `decimal` para valores monetários                
       92  
       50 -**Contexto:** <o que forçou a escolha>                        
       51 -**Decisão:** <o que foi decidido>                             
       52 -**Alternativa descartada:** <e por quê>                       
       53 -**Consequência:** <o que isso torna fácil e o que torna difíci
          -l>                                                            
       93 +**Contexto:** a spec exige somas, saldos e total exatos ao cen
          +tavo, com                                                     
       94 +arredondamento bancário (RN-009, RN-007, RN-011).             
       95 +**Decisão:** todo valor monetário é `decimal`; arredondamento 
          +com                                                           
       96 +`Math.Round(v, 2, MidpointRounding.ToEven)`; leitura do JSON c
          +om `GetDecimal()`.                                            
       97 +**Alternativa descartada:** `double` — representação binária i
          +nexata; 33,333 ou                                             
       98 +somas de saldos poderiam divergir em centavos e o teste de emp
          +ate (10,125 → 10,12)                                          
       99 +ficaria sujeito a erro de representação.                      
      100 +**Consequência:** cálculos exatos e testáveis por igualdade; o
          + custo é desempenho                                           
      101 +marginal, irrelevante para um colaborador por arquivo.        
      102  
       55 -### DT-002 — ...                                              
      103 +### DT-002 — Pipeline de passos em vez de `if` aninhado       
      104  
      105 +**Contexto:** a ordem de aplicação (seção 8) muda o resultado:
          + item recusado ou em                                          
      106 +revisão não consome saldo, não entra na comparação de duplicat
          +as e não conta como                                           
      107 +viagem.                                                       
      108 +**Decisão:** uma interface de passo (`Contexto Aplicar(Context
          +o)`) com um tipo por                                          
      109 +passo da seção 8, compostos numa lista ordenada. Cada passo só
          + olha itens `Ativo`.                                          
      110 +**Alternativa descartada:** um método único com `if` aninhados
          + por despesa — mistura                                        
      111 +regras que são de lista (duplicata, viagem, saldo diário) com 
          +regras de item, e a                                           
      112 +precedência fica implícita na indentação.                     
      113 +**Consequência:** a precedência é a própria lista; cada passo 
          +é testável isolado e                                          
      114 +mapeia para uma RN. Mudar a ordem é mudar uma linha, mas exige
          + alterar antes a                                              
      115 +seção 8 da spec.                                              
      116 +                                                              
      117 +### DT-003 — Política em `record` imutável injetado           
      118 +                                                              
      119 +**Contexto:** os limites mudam se a política mudar, e os teste
          +s precisam variá-los                                          
      120 +sem tocar os passos.                                          
      121 +**Decisão:** `PoliticaReembolso` como `record` com instância `
          +Padrao`, passada                                              
      122 +pelo construtor aos passos que a usam (seção 4 deste plano).  
      123 +**Alternativa descartada:** constantes estáticas espalhadas no
          +s passos — o valor                                            
      124 +fica duplicado e não é possível testar com outro limite.      
      125 +**Consequência:** um único ponto de mudança; o custo é um parâ
          +metro de construtor a                                         
      126 +mais.                                                         
      127 +                                                              
      128 +### DT-004 — `System.Text.Json` com ordem de propriedades fixa
      129 +                                                              
      130 +**Contexto:** a seção 9 da spec exige que duas execuções com a
          + mesma entrada                                                
      131 +produzam a mesma saída, e a seção 4 define a ordem dos campos 
          +e números com 2 casas.                                        
      132 +**Decisão:** DTOs de saída próprios, com `[JsonPropertyOrder]`
          + explícito em todos os                                        
      133 +campos, nomes em `snake_case` conforme a spec, e um `JsonConve
          +rter<decimal>` que                                            
      134 +escreve sempre 2 casas (`72.50`, `340.00`, `0.00`). A saída nã
          +o depende de                                                  
      135 +dicionários, de cultura (usa `InvariantCulture`) nem de data/h
          +ora atual.                                                    
      136 +**Alternativa descartada:** serializar o modelo de domínio dir
          +eto — a ordem e o                                             
      137 +formato passariam a depender de detalhes internos, e `decimal`
          + de escala 0 sairia                                           
      138 +como `100` em vez de `100.00`.                                
      139 +**Consequência:** saída byte a byte reproduzível, testável por
          + comparação de texto;                                         
      140 +o custo é uma camada de DTOs na `Cli`.                        
      141 +                                                              
      142  ## 6. Estratégia de testes
      143  
       59 -- **Nível:** <unitário, integração, ponta a ponta — e a propor
          -ção entre eles>                                               
       60 -- **Cada `RN-NNN` da spec tem teste?** <como você garante isso
          ->                                                             
       61 -- **Casos de borda da seção 7 da spec:** <cobertos como>      
       62 -- **Nomenclatura:** <como o nome do teste remete ao requisito 
          -— isso é o que                                                
       63 -  fecha a rastreabilidade na correção>                        
      144 +- **Nível:** predominantemente unitário (cada passo do pipelin
          +e isolado, no                                                 
      145 +  `Core`); poucos testes de composição do pipeline; um único t
          +este ponta a ponta                                            
      146 +  com `exemplos/despesas-exemplo.json` passando pela `Cli`.   
      147 +- **Cada `RN-NNN` da spec tem teste?** Sim: ao menos um teste 
          +por RN (RN-001 a                                              
      148 +  RN-011). A tabela de cobertura do `tasks.md` liga RN → task 
          +→ teste, e um teste                                           
      149 +  sem RN, ou RN sem teste, é lacuna visível ali.              
      150 +- **Casos de borda da seção 7 da spec:** um teste por linha da
          + tabela (14 linhas).                                          
      151 +- **Critérios de aceite da seção 9:** um teste por cenário mín
          +imo (total 581,53 e                                           
      152 +  `viagem` verdadeiro; itens aprovados; recusados e em revisão
          + com 0,00; resumo e                                           
      153 +  14 itens na ordem; d-001/d-002, d-003 e d-014 sozinhas; sem 
          +d-010 e d-013 a                                               
      154 +  `viagem` é falsa; justificativa e regras existentes em todo 
          +item; execução                                                
      155 +  repetida idêntica).                                         
      156 +- **Nomenclatura:** `RNxxx_Cenario_Resultado` (ex.:           
      157 +  `RN004_ValorExatamente100SemNota_NaoRecusa`) e `AMBxxx_...` 
          +quando o teste                                                
      158 +  exercita uma decisão da seção 6. O prefixo fecha a rastreabi
          +lidade com a spec.                                            
      159  
      160  ## 7. Riscos
      161  
      162  | Risco | Probabilidade | O que faço se acontecer |
      163  |---|---|---|
       69 -| | | |                                                       
      164 +| `decimal` sair no JSON sem 2 casas (`100` em vez de `100.00`
          +) ou com cultura errada | Média | O conversor da DT-004 format
          +a sempre com 2 casas e `InvariantCulture`; o teste ponta a pon
          +ta compara o texto da saída. Se ainda falhar, escrever o númer
          +o bruto com `Utf8JsonWriter.WriteRawValue` |                  
      165 +| Passos acoplados por estado (viagem e saldo dependem do que 
          +os passos anteriores deixaram `Ativo`) levando a resultado err
          +ado quando a ordem é alterada | Média | Cada passo declara em 
          +teste o que lê e o que altera; teste de precedência (seção 8 d
          +a spec: duas despesas iguais sem nota > 100,00 são `recusado`,
          + não duplicatas). Se a ordem precisar mudar, corrigir antes a 
          +spec |                                                        
      166 +| Normalização de categoria divergente da RN-001 (acentos, híf
          +en, espaço) | Média | Normalizar por decomposição Unicode (For
          +mD) removendo marcas diacríticas e, depois, aplicar caixa e tr
          +oca de separadores; um teste por variação. Se surgir um caso n
          +ão coberto pela spec, registrar em `DECISIONS.md` antes de cod
          +ificar |                                                      
      167 +| Entrada inválida (campo obrigatório ausente, arquivo malform
          +ado, valor 0): a spec diz que não há regra definida (seção 10)
          + | Alta | Não inventar regra no código. A `Cli` falha com mens
          +agem técnica e código de saída diferente de zero somente para 
          +JSON ilegível; os demais casos ficam sem tratamento específico
          + até a spec decidir. Registrar a pendência e levar ao dono da 
          +spec |                                                        
      168 +| `inicio`/`fim` divergindo de `competencia` (AMB-011 em abert
          +o) | Baixa | Usar só `competencia`, conforme a spec; ignorar `
          +inicio`/`fim`. Se o RH exigir outro comportamento, é mudança d
          +e spec |                                                      
      169 +| Mudança de requisito no Dia 2 atingir mais de um passo | Méd
          +ia | Política injetada (DT-003) e um tipo por passo (DT-002) l
          +imitam a mudança; novas tasks continuam a numeração, sem renum
          +erar as antigas |                                             

● Preenchi specs/001-motor-reembolso/plan.md com o que você passou e não criei
  regra de negócio nova. Não rodei nada nem fiz commit. Quando quiser commitar,
  a mensagem seria docs(plan): ....

  O plano tem as 7 seções:
  - Stack e arquitetura: a stack está na tabela, com os descartes que você
    listou. A arquitetura é o pipeline de 9 passos em Reembolso.Core, na ordem
    da seção 8 da spec, com Reembolso.Cli como casca de I/O.
  - Modelo e política: o modelo de dados usa records imutáveis. A política é um
    record PoliticaReembolso com Padrao (60, 80, 90, 120, 250 e 100), injetado
    nos passos.
  - Decisões técnicas e testes: DT-001 a DT-004 estão como você descreveu. Os
    testes seguem a estratégia que você pediu, com a nomenclatura
    RNxxx_Cenario_Resultado, e o plano lista 14 linhas de bordas e 9 cenários de
    aceite.
  - Riscos: são 6, cada um com a ação se acontecer.

  Dois pontos para você olhar:
  - Ativo é um estado interno: acrescentei Ativo ao enum Situacao, só para o
    pipeline saber quais itens ainda participam dos passos seguintes. Ele não
    aparece na saída.
  - Escala do decimal: o decimal de escala 0 sai como 100 em vez de 100.00. Por
    isso a DT-004 inclui um JsonConverter<decimal> que escreve sempre 2 casas, o
    que a spec exige na seção 4.

  Uma pendência da própria spec, que não resolvi: a seção 10 diz que arquivo
  inválido, campo obrigatório ausente e valor zero não têm regra definida. O
  plano não inventa tratamento para isso. Está como risco de probabilidade alta,
  e a decisão é sua na spec.

  O tasks.md ainda é o template. Os nomes de teste e os ids T-xxx da tabela de
  cobertura dependem dele.

✻ Baked for 1m 2s · done 18:41

※ recap: Estamos montando o motor de reembolso em SDD, e acabei de preencher o plan.md sem criar regra nova. Próxima ação: você revisar o plano e decidir o que fazer com os casos sem regra da seção 10 da spec, para então preenchermos o tasks.md.