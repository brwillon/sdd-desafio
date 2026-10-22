 ▐▛███▛█   Claude Code v2.1.308
▝▜██████▀  Sonnet 5.5 · Claude Pro
 ▝▝   ▝▝   D:\github\sdd-desafio


❯ /clear                                                                        

❯ Leia README.md, DESAFIO.md, RUBRICA.md, FAQ.md e                              
  exemplos/despesas-exemplo.json.                                               
  Resuma: o que é entregável, como cada critério é pontuado, as regras e o      
  cronograma.                                                                   
  Não proponha solução nem interprete a política ainda.                         

  Read 5 files (ctrl+o to expand)

● Resumo do desafio SDD

  O que é

  Desafio individual de 2 dias, feito no Claude Code, para construir um motor de
  reembolso de despesas. É uma CLI: <comando> calcular --input despesas.json 
  --output resultado.json. A stack é livre. O formato de entrada está fixado em
  exemplos/despesas-exemplo.json. O schema de saída é definido por você e
  documentado na spec. Testes automatizados são obrigatórios.

  O foco não é o código. É especificar uma política de RH ambígua, decidir cada
  ambiguidade, justificá-la e registrá-la. O produto vale 10 dos 100 pontos.

  Entregáveis

  Fork público do repositório, com:
  - CLAUDE.md (convenções para o agente).
  - README.md do seu projeto, que substitui o atual e explica como rodar e
    testar.
  - specs/001-motor-reembolso/, com quatro arquivos:
    - spec.md: o quê e o porquê. Contém regras desambiguadas, casos de borda,
      critérios de aceite e escopo negativo. Não pode ter biblioteca, classe,
      estrutura de pastas nem linguagem.
    - plan.md: o como. Contém stack, arquitetura, modelo de dados, alternativas
      descartadas e estratégia de testes.
    - tasks.md: tasks T-001..T-0NN. Cada uma traz o que faz, os requisitos que
      atende e o critério de aceite.
    - DECISIONS.md: log de toda mudança de spec (o que mudou, por quê, o que
      quebrou, tasks afetadas).
  - src/ e tests/.
  - docs/sessions/: um export do /export por sessão, nomeado NN-descricao.md.
  - docs/RELATORIO.md: o relatório final.

  Preparação: copiar template/CLAUDE.md, template/specs e template/docs para a
  raiz e fazer o commit inicial. A pasta template/ fica onde está. Entrega: o
  link do fork no formulário.

  Regras do jogo

  1. Nenhum commit sem task. Formato feat(T-003): ... e test(T-003): .... Para
     documentação, docs(spec|plan|tasks):. O git log é lido na correção.
  2. Regra explicada só no chat é bug de spec. Pare, corrija a spec, registre no
     DECISIONS.md e só então continue.
  3. Sessões exportadas e commitadas. Exporte ao final de cada sessão. Um único
     export gigante no último minuto é sinal de má-fé.

  Pontos práticos:
  - Teste o /export antes de começar. Se falhar, copie os .jsonl de
    ~/.claude/projects/ ou use o prompt de cópia do FAQ.
  - Não reescreva o histórico do git. Declare as lacunas no relatório.
  - Spec feita pelo Claude é esperada, mas as decisões são suas e você as
    defende no relatório.

  Política do RH (texto literal, v3)

  1. Alimentação: R$ 60 por dia.
  2. Transporte urbano: R$ 80 por dia.
  3. Hospedagem: R$ 250 por diária.
  4. Despesas acima do limite são reembolsadas parcialmente.
  5. Nota fiscal é obrigatória acima de R$ 100.
  6. Colaborador em viagem tem limites ampliados em 50%.
  7. Despesas devem ser lançadas dentro do período de competência.
  8. Duplicatas devem ser tratadas.
  9. Categorias fora da política não são reembolsáveis.

  Há no mínimo 8 ambiguidades. A lista de referência não é publicada. Elas são
  de três tipos: unidade de aplicação, fronteira e dado ausente. Todas são
  alcançáveis cruzando a política com o JSON de exemplo.

  Pontuação (100 pts)

  Critério: Qualidade da spec
  Pts: 25
  O que pontua: Oito ambiguidades decididas e justificadas, sem solução vazada,
  critérios verificáveis, escopo negativo. Faixas: 23–25, 17–22, 9–16, 0–8.
  ────────────────────────────────────────
  Critério: Rastreabilidade
  Pts: 25
  O que pontua: Cadeia spec → tasks → commits → testes. Procuram granularidade
  compatível, datas coerentes, testes nomeados por requisito e tasks marcadas
  ao longo do caminho.
  ────────────────────────────────────────
  Critério: Mudança de requisito (Dia 2)
  Pts: 20
  O que pontua: A mudança entra pela spec, com DECISIONS.md, novas tasks e diff
  cirúrgico. Bônus de até +3 (teto 20) se o relatório mostrar com números
  quanto veio de reexecutar tasks versus edição manual.
  ────────────────────────────────────────
  Critério: Relatório
  Pts: 20
  O que pontua: Cinco blocos (4 Ds + envelope) com evidência concreta.
  ────────────────────────────────────────
  Critério: Produto funciona
  Pts: 10
  O que pontua: Roda pelo README, processa o exemplo e o conjunto oculto, e a
  saída bate com a spec do próprio aluno.

  Zeros e limites:
  - Spec sem nenhuma ambiguidade zera o critério de spec.
  - Sem sessões exportadas, o critério de relatório inteiro zera. Com uma única
    sessão, o teto é 12.
  - Discernimento sem um erro concreto do Claude zera a seção e custa −8.

  Penalidades transversais:
  - Regra só no chat: −5 por ocorrência, teto −15.
  - DECISIONS.md ausente com spec alterada: −5.
  - CLAUDE.md ausente: −3.
  - README que não roda o projeto: −3.
  - Commit único sem histórico: −15, e a rastreabilidade fica limitada a 8.

  Não premia: elegância do código, quantidade de features, cobertura como
  número, tamanho da spec. Ambiguidade resolvida com uma decisão ruim, mas
  escrita e justificada, pontua integralmente. A resolução silenciosa no código
  é o que não pontua.

  Relatório (RELATORIO.md)

  - Delegação: o que foi seu e o que foi do Claude, onde houve arrependimento, e
    o uso de subagentes, skills, MCP e hooks.
  - Descrição: primeira e última versão de um requisito ambíguo, com histórico.
  - Discernimento: pelo menos um erro concreto do Claude, com o que ele propôs,
    por que estava errado, como você detectou, o que fez e o link da sessão.
  - Diligência: o que você verificou e o que aceitou sem verificar.
  - Envelope: arquivos tocados à mão, tempo gasto, o que da spec ajudou ou
    atrapalhou, e o que você faria diferente.
  - Honestidade conta. "Tudo correu bem" é avaliado como falta de atenção.

  Cronograma

  ┌───────────┬─────────────────────────────────────────────────────────────┐
  │  Quando   │                            O quê                            │
  ├───────────┼─────────────────────────────────────────────────────────────┤
  │ Dia 1,    │ spec → plan → tasks, sem código. Feche a spec até o         │
  │ manhã     │ meio-dia, mesmo imperfeita.                                 │
  ├───────────┼─────────────────────────────────────────────────────────────┤
  │ Dia 1,    │ Implementação guiada pelas tasks, com commits rastreáveis.  │
  │ tarde     │                                                             │
  ├───────────┼─────────────────────────────────────────────────────────────┤
  │ Dia 2,    │ Chega a mudança de requisito (envelope), pelo canal da      │
  │ ~10h      │ turma. É obrigatória e vale 20 pts.                         │
  ├───────────┼─────────────────────────────────────────────────────────────┤
  │ Dia 2,    │ Absorver a mudança, fechar os testes e escrever o           │
  │ tarde     │ relatório.                                                  │
  ├───────────┼─────────────────────────────────────────────────────────────┤
  │ Dia 2,    │ Entrega do link do fork.                                    │
  │ 18h       │                                                             │
  └───────────┴─────────────────────────────────────────────────────────────┘

  Atraso deve ser declarado no formulário. Ordem na absorção da mudança: spec →
  DECISIONS.md → tasks → código. Meia implementação com a spec correta vale mais
  que a implementação completa com a spec desatualizada.

  Entrada de exemplo (despesas-exemplo.json)

  Estrutura: colaborador (id, nome, centro_custo), periodo (competencia 2026-07,
  inicio e fim) e uma lista de despesas. Cada despesa tem id, data, categoria,
  descricao, fornecedor, valor e tem_nota_fiscal.

  São 14 itens, d-001 a d-014. Registro apenas o que está nos dados, sem
  interpretar a política:

  ┌────────────┬────────────────────────────────────────────────────────────┐
  │     Id     │                      Dado observável                       │
  ├────────────┼────────────────────────────────────────────────────────────┤
  │ d-001,     │ Alimentação no mesmo dia (03/07): 72,50 e 38,00.           │
  │ d-002      │                                                            │
  ├────────────┼────────────────────────────────────────────────────────────┤
  │ d-003,     │ Transporte no mesmo dia, sem nota fiscal: 100,00 e 100,01. │
  │ d-004      │                                                            │
  ├────────────┼────────────────────────────────────────────────────────────┤
  │ d-005      │ Categoria coworking, 89,00.                                │
  ├────────────┼────────────────────────────────────────────────────────────┤
  │ d-006,     │ Registros idênticos (mesmo dia, fornecedor e valor 54,90), │
  │ d-007      │  com ids diferentes.                                       │
  ├────────────┼────────────────────────────────────────────────────────────┤
  │ d-008      │ Data 2026-04-15, fora da competência 2026-07.              │
  ├────────────┼────────────────────────────────────────────────────────────┤
  │ d-009      │ Valor negativo (−45,00), descrito como estorno.            │
  ├────────────┼────────────────────────────────────────────────────────────┤
  │ d-010      │ Hospedagem de 480,00 descrita como "2 diarias".            │
  ├────────────┼────────────────────────────────────────────────────────────┤
  │ d-011      │ Valor com três casas decimais (33,333).                    │
  ├────────────┼────────────────────────────────────────────────────────────┤
  │ d-012      │ Descrição "sabado - plantao".                              │
  ├────────────┼────────────────────────────────────────────────────────────┤
  │ d-013      │ Hospedagem de 690,00 descrita como "3 noites", sem nota    │
  │            │ fiscal.                                                    │
  ├────────────┼────────────────────────────────────────────────────────────┤
  │ d-014      │ Categoria escrita como ALIMENTACAO, em maiúsculas, valor   │
  │            │ 61,00.                                                     │
  └────────────┴────────────────────────────────────────────────────────────┘

  Não há campo de viagem na entrada. O arquivo não traz nenhum dado explícito
  sobre ele.

  Observação sobre o CLAUDE.md atual

  O CLAUDE.md do repositório ainda tem placeholders <...> em stack, comandos,
  convenções de código e escopo. Ele exige ler a task em tasks.md antes de
  implementar e parar se você explicar uma regra que não está na spec. Ainda não
  li template/, nem specs/ ou docs/.

✻ Cogitated for 24s · done 11:13