 ▐▛███▛█   Claude Code v2.1.308
▝▜██████▀  Sonnet 5.5 · Claude Pro
 ▝▝   ▝▝   D:\github\sdd-desafio


❯ /clear                                                                        

❯ Leia DESAFIO.md (seção "A política de reembolso"), RUBRICA.md (critério 1) e  
  exemplos/despesas-exemplo.json.                                               
  Percorra o JSON item por item contra a política e liste TODAS as              
  ambiguidades, classificando cada uma como: unidade de aplicação, fronteira ou 
  dado ausente.                                                                 
  Para cada uma: cite o texto do RH, os ids de despesa que a expõem e as duas   
  ou mais leituras possíveis.                                                   
  NÃO decida nada e NÃO escreva na spec.                                        

  Read 3 files (ctrl+o to expand)

● Ambiguidades da política v3 × despesas-exemplo.json

  Nada foi decidido e nada foi escrito na spec. Encontrei 19 ambiguidades: 8 de
  unidade de aplicação (U), 5 de fronteira (F) e 6 de dado ausente (D).

  Em exemplos/despesas-exemplo.json, 2026-07-18 cai num sábado.

  ---

  Unidade de aplicação

  U1. "Por dia" ou por despesa?
  - Texto do RH: "Alimentação tem limite de R$ 60 por dia" e "Transporte urbano
    tem limite de R$ 80 por dia" (itens 1 e 2).
  - Ids: d-001 e d-002 (72,50 + 38,00 no mesmo dia), d-006 e d-007 (54,90 +
    54,90), d-003 e d-004 (100,00 + 100,01).
  - Leituras:
    - (a) O teto vale para a soma da categoria no dia. Os pares de d-001/d-002 e
      d-006/d-007 ficam limitados a 60, e o de d-003/d-004 a 80.
    - (b) O teto vale para cada despesa. d-001 paga 60 e d-002 paga 38. d-006 e
      d-007 pagam 54,90 cada, num total de 109,80 em um dia.
    - (c) "Dia" é a data do calendário. Outra leitura seria "dia de trabalho" ou
      "dia de viagem" (ver U8).

  U2. Como distribuir o corte dentro do dia
  - Texto do RH: o item 4, "reembolsadas parcialmente", combinado com o "por
    dia".
  - Ids: os mesmos pares da U1.
  - Leituras, caso a U1 seja lida como (a):
    - (a) Ordem do arquivo, em que o primeiro item consome o saldo.
    - (b) Ordem por id.
    - (c) Rateio proporcional ao valor.
    - (d) Maior valor primeiro.
    - A entrada não tem hora da despesa, então não existe ordem cronológica
      dentro do dia (isso é também um dado ausente).
  - O resultado por item muda conforme a leitura, mesmo com o total do dia
    igual.

  U3. "Parcialmente" significa o quê?
  - Texto do RH: "Despesas acima do limite são reembolsadas parcialmente"
    (item 4).
  - Ids: d-001 (72,50), d-004 (100,01), d-014 (61,00), d-010 (480 se lido como
    uma diária, ver U4).
  - Leituras:
    - (a) Paga o limite e corta o excedente.
    - (b) Recusa o item inteiro.
    - (c) Paga uma fração fixa (por exemplo, proporcional).
  - O exemplo do desafio sugere (a), mas o texto do RH não diz.

  U4. Hospedagem: "por diária" aplicado a quê?
  - Texto do RH: "Hospedagem tem limite de R$ 250 por diária" (item 3).
  - Ids: d-010 ("Hotel Rio - 2 diarias", 480,00) e d-013 ("Airbnb 3 noites",
    690,00).
  - Leituras:
    - (a) O limite vale por diária, então o teto do item é 250 × nº de diárias.
      d-010 cabe (240 por diária) e d-013 cabe (230 por noite).
    - (b) O limite vale por lançamento. d-010 paga 250 e d-013 paga 250.
    - (c) "Noite" e "diária" são equivalentes, ou "diária" pode incluir o dia de
      saída.

  U5. Nota fiscal: aplicada a qual valor?
  - Texto do RH: "Nota fiscal é obrigatória acima de R$ 100" (item 5).
  - Ids: d-004 (100,01 sem nota), d-013 (690,00 sem nota). Sem efeito nos dados,
    mas expõem a leitura: d-001+d-002 (110,50) e d-006+d-007 (109,80).
  - Leituras:
    - (a) Vale para o valor de cada despesa.
    - (b) Vale para a soma do dia ou da categoria. Isso fecharia a brecha de
      fracionar um gasto grande em vários pequenos.
    - (c) Vale para o valor depois do corte do limite. d-004 fica em 80, que é
      menor que 100, e a nota deixaria de ser exigida.
    - (d) Vale para o valor original, antes de qualquer corte.

  U6. A ampliação de 50% vale para quais limites?
  - Texto do RH: "Colaborador em viagem tem limites ampliados em 50%" (item 6).
  - Ids: d-010 e d-013 (hospedagem), d-003 e d-004 (transporte), d-011
    (alimentação).
  - Leituras:
    - (a) Todos os três tetos por dia: 90, 120 e 375.
    - (b) Só alimentação e transporte, com hospedagem de fora.
    - (c) O limiar de R$ 100 da nota também é um "limite" e passa a 150 em
      viagem. Isso muda d-004, que fica sem nota exigida.

  U7. O que é duplicata?
  - Texto do RH: "Duplicatas devem ser tratadas" (item 8).
  - Ids: d-006 e d-007 (iguais em tudo, exceto o id). Contra-exemplos que não
    deveriam colidir: d-001/d-002 (fornecedores diferentes) e d-003/d-004
    (descrições e valores diferentes).
  - Leituras sobre quais campos precisam coincidir:
    - (a) Data, categoria, fornecedor e valor.
    - (b) Todos os campos menos o id.
    - (c) Só fornecedor e valor.
    - (d) Valor aproximado, o que tornaria d-003/d-004 quase-duplicatas.
  - Também há a possibilidade de dois almoços legítimos iguais no mesmo dia.

  U8. "Tratadas" como, e em que ordem?
  - Texto do RH: o item 8.
  - Ids: d-006 e d-007.
  - Leituras:
    - (a) Descarta a segunda ocorrência e paga a primeira.
    - (b) Recusa as duas.
    - (c) Paga as duas e só sinaliza.
    - (d) Manda para revisão humana.
  - Há também a questão de qual das duas é "a segunda" (id? ordem no arquivo?).
  - A ordem das etapas também importa. Se a duplicata sai antes do limite
    diário, d-006 paga 54,90. Se sai depois, a soma 109,80 é cortada a 60.

  ---

  Fronteira

  F1. Nota fiscal com exatamente R$ 100,00
  - Texto do RH: "acima de R$ 100" (item 5).
  - Ids: d-003 (100,00 sem nota) e d-004 (100,01 sem nota).
  - Leituras:
    - (a) "Acima" é estrito, então d-003 não precisa de nota e d-004 precisa.
    - (b) Vale ≥ 100, e as duas precisam.
  - A diferença de um centavo parece ter sido posta de propósito.

  F2. Valor exatamente igual ao limite
  - Texto do RH: "acima do limite" (item 4).
  - Ids: nenhuma despesa do arquivo bate exatamente em 60, 80 ou 250. d-014
    (61,00) e d-001 (72,50) ficam acima, e d-010 e d-013 ficam abaixo por
    diária. A fronteira só aparece em casos ocultos.
  - Leituras:
    - (a) Igual ao limite está dentro e é pago integralmente.
    - (b) Igual ao limite já conta como excesso.

  F3. Extremos do período e fonte da verdade
  - Texto do RH: "dentro do período de competência" (item 7).
  - Ids: d-014 (2026-07-31, o último dia) e d-008 (2026-04-15, fora).
  - Leituras:
    - (a) inicio e fim são inclusivos, então d-014 entra.
    - (b) fim é exclusivo.
    - (c) A fonte é o campo competencia ("2026-07") e não inicio/fim. Se os dois
      divergirem, não há regra.

  F4. Arredondamento
  - Texto do RH: não diz nada sobre centavos.
  - Ids: d-011 (33,333).
  - Leituras:
    - (a) Arredonda para cima a partir de 0,005 (half-up), o que dá 33,33.
    - (b) Trunca para 33,33.
    - (c) Arredondamento bancário.
    - (d) Rejeita a entrada por ter mais de 2 casas decimais.
  - Quando arredondar também é aberto: por item, por dia ou no total. Outra
    dúvida é se um valor com 3 casas é erro de dado.

  F5. Rótulo da categoria em maiúsculas
  - Texto do RH: "Categorias fora da política não são reembolsáveis" (item 9).
  - Ids: d-014 ("ALIMENTACAO"), contra "alimentacao" nos demais.
  - Leituras:
    - (a) A comparação ignora maiúsculas e minúsculas, então d-014 é alimentação
      e entra no limite (61 > 60).
    - (b) A comparação é exata, então d-014 está fora da política e não é pago.
    - (c) É um erro de dado a ser rejeitado.
  - Outras variações de grafia (acentos, espaços) estão no mesmo grupo.

  ---

  Dado ausente

  D1. O que é "em viagem"
  - Texto do RH: "Colaborador em viagem" (item 6).
  - Ids: d-003 e d-004 ("aeroporto" e "hotel", sem hospedagem no mesmo dia),
    d-010 (2 diárias a partir de 07-14), d-011 (07-15, café no hotel), d-013 (3
    noites a partir de 07-22), d-001/d-002/d-012 (sem indício).
  - Leituras:
    - (a) Não existe o conceito, porque a entrada não tem o campo. A ampliação
      nunca é aplicada.
    - (b) O dia está coberto pelas diárias de uma hospedagem. d-011 estaria em
      viagem e d-003/d-004 não.
    - (c) O período inteiro ou toda a competência vira "viagem" se houver
      qualquer hospedagem.
    - (d) A viagem é inferida por palavras-chave na descrição ou no fornecedor
      ("aeroporto", "hotel").
    - (e) A entrada ganha um campo novo.

  D2. Número de diárias só no texto livre
  - Texto do RH: "por diária" (item 3).
  - Ids: d-010 ("2 diarias") e d-013 ("3 noites").
  - Leituras:
    - (a) Extrai o número da descrição.
    - (b) Assume 1 diária quando a descrição não for confiável.
    - (c) Calcula pelo valor ou pelas datas.
  - A data é única por lançamento, então não dá para saber o intervalo. Os dias
    cobertos também alimentam a D1.

  D3. Data da despesa × data do lançamento
  - Texto do RH: "Despesas devem ser lançadas dentro do período" (item 7).
  - Ids: d-008 (data 2026-04-15, descrição "lançado com atraso").
  - Leituras:
    - (a) Vale a data da despesa, então d-008 fica fora e é recusada.
    - (b) Vale a data de lançamento, que não existe na entrada. Aí d-008,
      lançada em julho, estaria dentro. O texto "lançado com atraso" sugere que
      o lançamento foi em julho.
    - (c) Aceita fora do período com exceção, o que exigiria um campo de
      justificativa que também não existe.

  D4. Estorno
  - Texto do RH: a política não menciona valor negativo.
  - Ids: d-009 (−45,00, "Estorno de corrida cancelada", 07-11, sem nota).
  - Leituras:
    - (a) Ignora, por não ser uma despesa.
    - (b) Abate do total reembolsável.
    - (c) Abate da soma diária da categoria em 07-11, sem nenhuma outra corrida
      nesse dia.
    - (d) Referencia uma corrida original que o arquivo não identifica. As
      corridas são de 07-06 (d-003/d-004).
    - (e) É um dado inválido, rejeitado.
  - Pontos em aberto: o total pode ficar negativo? A comparação com o limite
    (−45 < 80) e a regra da nota (−45 < 100) valem para valores negativos?

  D5. Categoria sem mapeamento
  - Texto do RH: "Categorias fora da política não são reembolsáveis" (item 9),
    com só três categorias nomeadas.
  - Ids: d-005 (coworking, 89,00, com nota, "Diaria em espaco compartilhado").
  - Leituras:
    - (a) A lista fechada é alimentação, transporte urbano e hospedagem, então
      coworking está fora e vale 0.
    - (b) É uma espécie de hospedagem, pela palavra "diária".
    - (c) É uma categoria desconhecida enviada para revisão.
  - Também não está dito como a recusa aparece na saída (valor zero com
    justificativa, ou item omitido).

  D6. Fim de semana e plantão
  - Texto do RH: o item 1 diz "por dia", sem dizer dia útil ou calendário.
  - Ids: d-012 ("Almoco de sabado - plantao", 47,20, sábado).
  - Leituras:
    - (a) É um dia como qualquer outro e o limite normal vale.
    - (b) Não existe regra de fim de semana na política, então ele é pago
      normalmente.
    - (c) "Plantão" é um tipo de trabalho especial e sem regra explícita, e
      poderia contar como viagem ou ampliação.
  - Não muda o valor neste caso, porque 47,20 < 60. Só aparece em casos ocultos.

  ---

  Interação entre regras (cruza vários itens acima)

  Precedência: nota fiscal × limite.
  - Texto do RH: os itens 4 e 5.
  - Ids: d-004 (100,01 sem nota, acima de 80) e d-013 (690 sem nota, dentro do
    limite).
  - Leituras:
    - (a) Sem nota, recusa total (zera o item), mesmo que o limite permitisse
      pagar algo.
    - (b) Limite primeiro e nota depois (ver U5 (c)).
    - (c) Sem nota, paga até o menor valor que dispensaria a nota.
  - Também falta definir a ordem geral de aplicação de período, categoria,
    duplicata, limite, nota e viagem, porque ela muda o resultado de d-004,
    d-006/d-007 e d-013.

  ---

  Cobertura por id

  ┌──────────────┬─────────────────────────────────────────────────────┐
  │      Id      │               Ambiguidades que expõe                │
  ├──────────────┼─────────────────────────────────────────────────────┤
  │ d-001, d-002 │ U1, U2, U3, U5, D1                                  │
  ├──────────────┼─────────────────────────────────────────────────────┤
  │ d-003        │ F1, U1, U2, U6, D1                                  │
  ├──────────────┼─────────────────────────────────────────────────────┤
  │ d-004        │ F1, U1, U2, U3, U5, U6, D1, interação nota × limite │
  ├──────────────┼─────────────────────────────────────────────────────┤
  │ d-005        │ D5                                                  │
  ├──────────────┼─────────────────────────────────────────────────────┤
  │ d-006, d-007 │ U1, U2, U7, U8, U5                                  │
  ├──────────────┼─────────────────────────────────────────────────────┤
  │ d-008        │ F3, D3                                              │
  ├──────────────┼─────────────────────────────────────────────────────┤
  │ d-009        │ D4                                                  │
  ├──────────────┼─────────────────────────────────────────────────────┤
  │ d-010        │ U4, D2, U6, D1                                      │
  ├──────────────┼─────────────────────────────────────────────────────┤
  │ d-011        │ F4, D1                                              │
  ├──────────────┼─────────────────────────────────────────────────────┤
  │ d-012        │ D6                                                  │
  ├──────────────┼─────────────────────────────────────────────────────┤
  │ d-013        │ U4, D2, U5, U6, D1, interação nota × limite         │
  ├──────────────┼─────────────────────────────────────────────────────┤
  │ d-014        │ F5, F3, U3                                          │
  └──────────────┴─────────────────────────────────────────────────────┘

  Nenhum id ficou sem ambiguidade. Os itens com maior chance de estarem na lista
  oculta do RUBRICA são os que têm um id dedicado: F1 (d-003/d-004), D3
  (d-008), D4 (d-009), F4 (d-011), F5 (d-014), D5 (d-005), U7/U8 (d-006/d-007),
  U4/D2 (d-010/d-013) e D1.

  Isso é meu palpite. A rubrica não publica a lista.

✻ Baked for 1m 23s · done 11:22

❯ Para cada despesa d-001..d-014, diga qual ambiguidade da lista ela exercita.  
  Alguma despesa ficou sem ambiguidade associada? O que ela testa que ainda não 
  listamos?                                                                     

● Nenhuma despesa ficou sem ambiguidade associada, mas duas ficaram só com
  associações fracas. A tabela mostra quais, e depois listo o que elas testam e
  eu não tinha listado.

  Despesa → ambiguidade

  ┌─────────────────────┬────────────────────────────┬─────────────────────┐
  │         Id          │        Ambiguidades        │        Força        │
  ├─────────────────────┼────────────────────────────┼─────────────────────┤
  │ d-001 (72,50)       │ U1, U2, U3, U5 (soma do    │ Forte               │
  │                     │ dia 110,50), D1            │                     │
  ├─────────────────────┼────────────────────────────┼─────────────────────┤
  │                     │ U1, U2, U5, D1 (é o        │                     │
  │ d-002 (38,00)       │ segundo item do dia de     │ Forte               │
  │                     │ d-001)                     │                     │
  ├─────────────────────┼────────────────────────────┼─────────────────────┤
  │ d-003 (100,00)      │ F1 (nota exatamente em     │ Forte               │
  │                     │ 100), U1, U2, U6, D1       │                     │
  ├─────────────────────┼────────────────────────────┼─────────────────────┤
  │                     │ F1, U1, U2, U3, U5, U6,    │                     │
  │ d-004 (100,01)      │ D1, interação nota ×       │ Forte               │
  │                     │ limite                     │                     │
  ├─────────────────────┼────────────────────────────┼─────────────────────┤
  │ d-005 (coworking)   │ D5                         │ Forte               │
  ├─────────────────────┼────────────────────────────┼─────────────────────┤
  │ d-006 (54,90)       │ U7, U8, U1, U2, U5 (soma   │ Forte               │
  │                     │ 109,80)                    │                     │
  ├─────────────────────┼────────────────────────────┼─────────────────────┤
  │ d-007 (54,90)       │ U7, U8, U1, U2, U5 (soma   │ Forte               │
  │                     │ 109,80)                    │                     │
  ├─────────────────────┼────────────────────────────┼─────────────────────┤
  │ d-008 (abril)       │ F3, D3                     │ Forte               │
  ├─────────────────────┼────────────────────────────┼─────────────────────┤
  │ d-009 (−45,00)      │ D4                         │ Forte               │
  ├─────────────────────┼────────────────────────────┼─────────────────────┤
  │ d-010 (480, 2       │ U4, D2, U6, D1             │ Forte               │
  │ diárias)            │                            │                     │
  ├─────────────────────┼────────────────────────────┼─────────────────────┤
  │                     │                            │ Média: o valor está │
  │ d-011 (33,333)      │ F4, D1                     │  sempre abaixo do   │
  │                     │                            │ limite              │
  ├─────────────────────┼────────────────────────────┼─────────────────────┤
  │ d-012 (sábado,      │ D6                         │ Fraca               │
  │ plantão)            │                            │                     │
  ├─────────────────────┼────────────────────────────┼─────────────────────┤
  │ d-013 (690, 3       │ U4, D2, U5, U6, D1,        │ Forte               │
  │ noites)             │ interação nota × limite    │                     │
  ├─────────────────────┼────────────────────────────┼─────────────────────┤
  │ d-014               │ F5, F3 (último dia do      │                     │
  │ ("ALIMENTACAO",     │ período), U3               │ Forte               │
  │ 61,00)              │                            │                     │
  └─────────────────────┴────────────────────────────┴─────────────────────┘

  Quem ficou sem associação sólida

  d-012. A D6 (fim de semana e plantão) foi a menos fundamentada da lista. O
  valor é 47,20, abaixo de 60, então nenhuma leitura muda o resultado. O que a
  linha provavelmente testa é outra coisa: um item limpo (categoria válida, com
  nota, dentro do período, abaixo do limite) que precisa ser pago integralmente.
  Isso é um caso de controle. Ele garante que o motor não recuse por engano, e
  não expõe ambiguidade nenhuma.

  Se existir um sentido real de "plantão" ou de dia da semana, ele depende de a
  política dizer algo sobre fim de semana. Ela não diz.

  d-011. A F4 (arredondamento) é real, mas o resto da linha tem pouco peso. O
  café da manhã no hotel levanta uma questão que ainda não listei, descrita
  abaixo como N2.

  O que as descrições testam e não estava na lista

  N1. Despesa com terceiros.
  - Ids: d-001 ("Almoço com cliente") e d-014 ("Jantar de encerramento").
  - O limite de R$ 60 vale por colaborador ou por refeição?
  - Leituras:
    - (a) Vale por colaborador, então o almoço de 72,50 pode ter coberto mais de
      uma pessoa.
    - (b) Vale por refeição, com cliente ou sem.
    - (c) Refeição com cliente é representação, uma categoria fora da política,
      e vale 0.
  - A entrada não traz número de pessoas nem motivo. É um dado ausente.

  N2. Sobreposição entre categorias na mesma estadia.
  - Ids: d-011 (café da manhã no Hotel Copa Sul, em 07-15) e d-010 (a diária do
    mesmo hotel, 07-14 a 07-16).
  - O café pode já estar incluído na diária de 480. Nesse caso haveria pagamento
    em duplicidade.
  - Leituras:
    - (a) São independentes, e o café entra no limite diário de alimentação.
    - (b) O café está embutido na diária e não é pago à parte.
  - Isso é diferente da duplicata da U7, porque os valores, as categorias e as
    descrições são diferentes.

  N3. Despesa de vários dias, com data única.
  - Ids: d-010 (2 diárias) e d-013 (3 noites).
  - A data do lançamento é uma só (07-14 e 07-22). A D2 cobriu o número de
    diárias, mas não a atribuição das datas.
  - Para o período (item 7), vale só a data do lançamento ou o intervalo
    inteiro?
  - Para a ampliação por viagem (D1), quais dias a hospedagem cobre?
  - Nos dados não muda nada, porque as duas hospedagens ficam inteiras em julho.
    Uma estadia que cruza a virada do mês, num caso oculto, mudaria.

  Resumo

  Com N1, N2 e N3 a lista sobe para 22. Dessas, N3 e a D6 não alteram valor nos
  dados atuais, e só importariam em casos ocultos. Das 14 linhas, 12 têm função
  clara. d-012 é o controle, sem ambiguidade real. d-011 testa o arredondamento
  e tem um segundo ângulo, o N2.

  Continuo sem decidir nada e sem escrever na spec.

✻ Cooked for 31s · done 11:24

※ recap: O objetivo é escrever a spec do motor de reembolso, e agora estamos só levantando as ambiguidades da política de RH contra o JSON de exemplo, sem decidir nada. Próximo passo: você escolher as interpretações de cada ambiguidade, para eu registrá-las na spec. (disable recaps in /config)

❯ Preencha specs/001-motor-reembolso/spec.md seguindo o template, usando SÓ as  
  decisões abaixo.                                                              
  Se faltar decisão para algo, pare e pergunte — não invente.                   
  U1. "Por dia" ou por despesa?                                                 
  AMB-001                                                                       
  Decisão: (a) O teto vale para a soma da categoria no dia. Os pares de         
  d-001/d-002 e d-006/d-007 ficam limitados a 60, e o de d-003/d-004 a 80.      
  Porque: porque a política não informa que a restrição é por refeição          
  realizado no dia, mas que é um limite diário.                                 
                                                                                
  U2. Como distribuir o corte dentro do dia                                     
  AMB-002                                                                       
  Decisão: (a) Ordem do arquivo, em que o primeiro item consome o saldo.        
  Porque: porque é um limite diário e existe um saldo disponível para consumo   
  que será aproveitado no lançamento seguinte.                                  
                                                                                
  U3. "Parcialmente" significa o quê?                                           
  AMB-003                                                                       
  Decisão: (a) Paga o limite e corta o excedente.                               
  Porque: O valor de limite representa o valor de direito reembolsável.         
                                                                                
  U4. Hospedagem: "por diária" aplicado a quê?                                  
  AMB-004                                                                       
  Decisão: (a) O limite vale por diária, então o teto do item é 250 × nº de     
  noites. d-010 cabe (240 por diária) e d-013 cabe (230 por noite).             
  Porque: Porque a diária de hospedagem conta o número de noites                
                                                                                
  U5. Nota fiscal: aplicada a qual valor?                                       
  AMB-005                                                                       
  Decisão:(a) Vale para o valor de cada despesa.                                
  Porque: despesas acima de R$ 100 requerem evidência para aprovação, mesmo que 
  o limite da diária esteja abaixo desse valor.                                 
                                                                                
  U6. A ampliação de 50% vale para quais limites?                               
  AMB-006                                                                       
  Decisão: (b) Só alimentação e transporte, com hospedagem de fora.             
  Porque: a hospedagem é a evidência para a ampliação do limite e não muda seu  
  limite para exigência da nota, que continua sendo de R$ 100.                  
                                                                                
  U7. O que é duplicata?                                                        
  AMB-007                                                                       
  Decisão: (a) Data, categoria, fornecedor e valor. Porém, devem ser tratadas   
  como advertência, as informações serão checadas posteriormente, ação de       
  checagem e validação que está fora deste escopo.                              
  Porque: Existe a possibilidade de consumo no mesmo dia, com o mesmo           
  fornecedor, categoria e valor.                                                
                                                                                
  U8. "Tratadas" como, e em que ordem?                                          
  AMB-008                                                                       
  Decisão: (d) Manda para revisão humana todas as duplicatas.                   
  Porque: Todos os itens identificados como duplicatas devem ser checados.      
                                                                                
  F1. Nota fiscal com exatamente R$ 100,00                                      
  AMB-009                                                                       
  Decisão: (a) "Acima" é estrito, então d-003 não precisa de nota e d-004       
  precisa.                                                                      
  Porque: Até 100 entendesse que é tolerado sem nota                            
                                                                                
  F2. Valor exatamente igual ao limite                                          
  AMB-010                                                                       
  Decisão: (a) Igual ao limite está dentro e é pago integralmente.              
  Porque: acima do limite que o pagamento será parcial de acordo com o saldo    
  ainda existente na categoria                                                  
                                                                                
  F3. Extremos do período e fonte da verdade                                    
  AMB-011                                                                       
  Decisão: (c) A fonte é o campo competencia ("2026-07") e não inicio/fim. Se   
  os dois divergirem, não há regra.                                             
  Porque: o reembolso está atrelado à competência informada                     
                                                                                
  F4. Arredondamento                                                            
  AMB-012                                                                       
  Decisão: (c) Arredondamento bancário, com 2 casas decimais.                   
  Porque: segue o modelo bancário e evita a divergência entre saldos            
                                                                                
  F5. Rótulo da categoria em maiúsculas                                         
  AMB-013                                                                       
  Decisão: A comparação ignora maiúsculas e minúsculas, então d-014 é           
  alimentação e entra no limite (61 > 60). Outras variações de grafia (acentos, 
  espaços) estão no mesmo grupo e precisão ser normalizados caso seja possível  
  a inferência nas categorias aceitas. Os demais casos, devem ser rejeitados.   
  Porque: categorias não identificadas devem ser rejeitas e encaminhadas para   
  análise humana.                                                               
                                                                                
  D1. O que é "em viagem"                                                       
  AMB-014                                                                       
  Decisão: (c) O período inteiro ou toda a competência vira "viagem" se houver  
  qualquer hospedagem.                                                          
  Porque: sem hospedagem caracteriza como deslocamento curto e não envolve      
  reembolso.                                                                    
                                                                                
  D2. Número de diárias só no texto livre                                       
  AMB-015                                                                       
  Decisão: a diária deve ser lançada individualmente                            
  Porque: caso tenha mais de uma no mesmo descritivo do lançamento, será        
  contabilizado apenas como uma diária.                                         
                                                                                
  D3. Data da despesa × data do lançamento                                      
  AMB-016                                                                       
  Decisão: Aceita fora do período com exceção.                                  
  Porque: a aprovação ou rejeição ficará a cargo do revisor humano              
                                                                                
  D4. Estorno                                                                   
  AMB-017                                                                       
  Decisão: (e) É um dado inválido, rejeitado.                                   
  Porque: é um processo de reembolso, entrada somente de valores reembolsáveis, 
  devoluções devem ser lanças em processo espefífico fora deste escopo.         
                                                                                
  D5. Categoria sem mapeamento                                                  
  AMB-018                                                                       
  Decisão: a) A lista fechada é alimentação, transporte urbano e hospedagem,    
  então coworking está fora e vale 0.                                           
  Porque: somente categorias reembolsáveis são válidas, as demais devem ser     
  rejeitadas                                                                    
                                                                                
  D6. Fim de semana e plantão                                                   
  AMB-019                                                                       
  Decisão: (a) É um dia como qualquer outro e o limite normal vale de acordo    
  com as demais regras.                                                         
  Porque: Finais de semana, feriados e plantões são tratados externamente como  
  horas-extras trabalhadas, sem mudança nos limtes de reembolso.                
                                                                                
  Precedência: nota fiscal × limite.                                            
  AMB-021                                                                       
  Decisão: (a) Sem nota, recusa total (zera o item), mesmo que o limite         
  permitisse pagar algo.                                                        
  Porque: a nota é obrigatória para aceitação                                   
                                                                                
  N1. Despesa com terceiros.                                                    
  AMB-022                                                                       
  Decisão: (c) Refeição com cliente é representação, uma categoria fora da      
  política, e vale 0.                                                           
  Porque: o reembolso da diária é exclusiva para o Colaborador                  
                                                                                
  N2. Sobreposição entre categorias na mesma estadia.                           
  AMB-023                                                                       
  Decisão: a) São independentes, e o café entra no limite diário de             
  alimentação, desde que não faça parte da nota de hospedagem, cuja checagem    
  será realizada no momento da validação humana, fora deste escopo.             
  Porque: Pode não estar incluída na diária de hospedagem                       
                                                                                
  N3. Despesa de vários dias, com data única.                                   
  AMB-024                                                                       
  Decisão: cada diária de hospedagem deve ser incluída de forma independente,   
  podendo ser anexada a mesma nota que será validada na checagem humana.        
  Porque: padroniza e facilita o processo                                       
                                                                                
  Restrições: nenhum nome de linguagem, biblioteca, classe ou pasta.            
  Cada RN com critério de aceite numérico usando os ids do exemplo. Inclua      
  schema de saída com exemplo, ordem de aplicação das regras (seção 8) e escopo 
  negativo (seção 3).                                                           

  Searched for 1 pattern, read 1 file (ctrl+o to expand)

● Não escrevi nada na spec. O spec.md ainda é o template vazio. Encontrei 4
  conflitos ou lacunas que mudam números de aceite, mais 4 detalhes menores.
  Pela sua regra ("se faltar decisão, pare e pergunte"), preciso das respostas
  antes de escrever.

  Bloqueantes

  1. AMB-004, AMB-015 e AMB-024 se contradizem sobre as diárias.
  - A AMB-004 diz que o teto é 250 × nº de noites, e que d-010 (480, "2
    diarias") cabe.
  - A AMB-015 diz que, se houver mais de uma diária no mesmo lançamento, conta
    como uma só. Com isso, d-010 tem teto 250 e paga 250, cortando 230.
  - A AMB-024 pressupõe um lançamento por diária.
  - Proposta: vale a AMB-015. Cada lançamento de hospedagem é uma diária, com
    teto de 250, e a frase "d-010 cabe" sai da AMB-004. Confirma ou prefere a
    AMB-004?

  2. O que acontece com os itens que vão para "revisão humana"?
  Isso cobre as duplicatas (AMB-008), as despesas fora do período (AMB-016) e as
  categorias não reconhecidas (AMB-013). Faltam estas decisões:
  - (a) Nas duplicatas, vão para revisão os dois itens do par ou só o segundo?
  - (b) O valor reembolsável do item em revisão é 0 (pendente), ou é calculado
    normalmente e só marcado com advertência? A AMB-007 diz "advertência" e a
    AMB-008 diz "revisão humana".
  - (c) O item em revisão consome saldo do dia? Entra no total? Conta como
    evidência de viagem?
  - (d) A AMB-013 manda categoria não identificada para análise humana. A
    AMB-018 diz que coworking (d-005) é rejeitado e vale 0. Coworking é rejeição
    final ou vai para revisão?
  - Proposta: três estados: aprovado (inclui parcial), recusado (final, valor 0)
    e em_revisao (valor 0 no total e sem consumir saldo). Os dois itens do par
    duplicado vão para revisão. Coworking é recusado. Categoria com grafia
    irreconhecível vai para em_revisao.

  3. Como identificar "refeição com cliente" (AMB-022)?
  - A entrada não tem campo para isso. Só existe a descrição em texto livre, e
    d-001 diz "Almoco com cliente".
  - Sem critério, o sistema não consegue aplicar a decisão.
  - Opções:
    - (a) Descrição contendo a palavra "cliente".
    - (b) A entrada ganha um campo novo, e d-001 deixa de ser detectado.
    - (c) Não há detecção automática, e a decisão vira orientação para a revisão
      humana.
  - Isso muda o resultado de d-001 (72,50), que passaria a valer 0.

  4. Hospedagem recusada conta como evidência de viagem (AMB-014)?
  - Exemplo: d-013 é recusada por falta de nota. Ela ainda caracteriza a
    competência como viagem?
  - No arquivo de exemplo isso não importa, porque d-010 é válida e já basta. Em
    casos ocultos importa.
  - Proposta: só conta hospedagem aprovada.

  Menores (se você só disser "ok", sigo a proposta)

  - AMB-020 não existe. A numeração salta de 019 para 021. Foi intencional ou
    falta uma decisão?
  - Quando arredondar (AMB-012). Proponho arredondar o valor de cada despesa
    para 2 casas na leitura, antes de somar saldos.
  - Estorno (AMB-017). Proponho recusar só o item negativo e processar o resto
    do arquivo.
  - Ordem de aplicação (seção 8). Você decidiu só "nota antes do limite".
    Proponho esta ordem:
    a. Valor inválido (estorno).
    b. Categoria.
    c. Período.
    d. Nota fiscal.
    e. Duplicata.
    f. Viagem.
    g. Limite diário com saldo na ordem do arquivo.
    h. Total.

    Confirma?
  - AMB-011. Você disse que, se competencia e inicio/fim divergirem, não há
    regra. Vou registrar isso na seção 10 como lacuna conhecida, sem inventar
    comportamento.

  Com as respostas, escrevo a spec inteira em um passo e deixo o commit como
  docs(spec) para você revisar.

✻ Baked for 59s · done 16:06

❯ 1. vale AMB-015                                                               

● Registrado: vale a AMB-015. Cada lançamento de hospedagem conta como uma
  diária, com teto de 250. Então d-010 paga 250 e corta 230, e a frase "d-010
  cabe" sai da AMB-004. Ainda não escrevi nada na spec.

  Faltam três bloqueantes. Sem eles não consigo fixar os números de aceite.

  - 2. Itens em revisão humana. Aceita a proposta?
    - Três estados: aprovado (inclui parcial), recusado (final, valor 0) e
      em_revisao (valor 0, sem consumir saldo).
    - Os dois itens de um par duplicado vão para revisão.
    - Coworking é recusado.
    - Categoria com grafia irreconhecível vai para em_revisao.
  - 3. "Refeição com cliente" (d-001). Escolha uma opção:
    - (a) A descrição contém "cliente".
    - (b) A entrada ganha um campo novo.
    - (c) Não há detecção automática, e fica como orientação para a revisão
      humana.
  - 4. Evidência de viagem. Só hospedagem aprovada caracteriza viagem (AMB-014)?
    Isso vale como proposto?

  Os menores seguem como propus se você disser "ok" para eles:
  - Arredondar cada valor para 2 casas na leitura.
  - Recusar só o item de estorno.
  - Ordem da seção 8: valor inválido, categoria, período, nota, duplicata,
    viagem, limite, total.
  - Registrar a divergência da AMB-011 como lacuna.

  Falta também esclarecer a AMB-020: foi pulada de propósito?

✻ Crunched for 7s · done 16:09

❯ 2. use os três estados recomendados, proposta aceita                          

● Registrado: valem os três estados (aprovado, recusado, em_revisao) e a
  proposta inteira do item 2. Ainda não escrevi nada na spec.

  Faltam duas respostas bloqueantes:

  - 3. "Refeição com cliente" (d-001). Como identificar? A decisão da AMB-022
    manda zerar a refeição com cliente, mas a entrada só traz o texto livre da
    descrição.
    - (a) A descrição contém "cliente". d-001 vale 0.
    - (b) A entrada ganha um campo novo. d-001 não é detectado e fica aprovado.
    - (c) Não há detecção automática. A decisão vira orientação para a revisão
      humana, e d-001 fica aprovado.
  - 4. Evidência de viagem (AMB-014). Só hospedagem aprovada caracteriza viagem?
    Ou conta também a recusada ou em revisão? Minha proposta é que só conte a
    aprovada.

  Os menores seguem como propus se você responder "ok":
  - Arredondar cada valor para 2 casas na leitura.
  - Recusar só o item de estorno.
  - Ordem da seção 8: valor inválido, categoria, período, nota, duplicata,
    viagem, limite, total.
  - Registrar a divergência da AMB-011 como lacuna.

  A AMB-020 continua sem resposta. Ela foi pulada de propósito?

✻ Cooked for 7s · done 16:11

❯ 3. Não há detecção automática. A decisão vira orientação para a revisão       
  humana, e d-001 fica aprovado.                                                

● Registrado: a AMB-022 vira orientação para a revisão humana, sem detecção
  automática. d-001 fica aprovado e entra normalmente no limite diário. Na spec
  isso aparece como escopo negativo (seção 3), e não como regra de negócio.
  Ainda não escrevi nada.

  Falta uma resposta bloqueante:

  - 4. Evidência de viagem (AMB-014). Só hospedagem aprovada caracteriza viagem?
    Minha proposta é que só conte a aprovada. Se a recusada ou a em revisão
    também contasse, uma hospedagem sem nota (como d-013) ampliaria os limites
    mesmo sem ser paga.

  Se você disser "ok" para os menores, sigo como propus:
  - Arredondar cada valor para 2 casas na leitura.
  - Recusar só o item de estorno.
  - Ordem da seção 8: valor inválido, categoria, período, nota, duplicata,
    viagem, limite, total.
  - Registrar a divergência da AMB-011 como lacuna na seção 10.

  A AMB-020 também continua sem resposta. Ela foi pulada de propósito?

✻ Brewed for 7s · done 16:12

❯ ok                                                                            

● Vou tratar o seu "ok" como aceite das propostas menores e da proposta do item
  4 (só hospedagem aprovada caracteriza viagem). Mantenho a numeração AMB-001 a
  019 e 021 a 024, como você mandou, e registro a lacuna da AMB-020 na seção 10.

● Write(specs\001-motor-reembolso\spec.md)
  ⎿  Added 430 lines, removed 33 lines
      1  # Spec — Motor de Cálculo de Reembolso
      2  
      3 -**Versão:** 1.0 · **Status:** rascunho · **Última alteração:** `
        -<data>`                                                         
      3 +**Versão:** 1.0 · **Status:** rascunho · **Última alteração:** 2
        +026-10-22                                                       
      4  
      5  > **Regra de ouro deste arquivo:** ele descreve o QUÊ e o PORQUÊ
         . Nenhuma linha
      6  > aqui pode citar linguagem, biblioteca, classe, função ou estru
         tura de pasta.
     ...
       13  
       14  ## 1. Problema
       15  
       16 -<Duas ou três frases. Quem sofre hoje, com o quê, e por quê is
          -so custa caro.>                                               
       16 +O financeiro confere manualmente, item por item, as despesas d
          +e cada colaborador                                            
       17 +contra a política de reembolso (v3). O processo é lento e prod
          +uz resultados                                                 
       18 +divergentes, porque a política admite mais de uma leitura em v
          +ários pontos e cada                                           
       19 +conferente resolve essas dúvidas por conta própria, sem regist
          +ro.                                                           
       20  
       21  ## 2. Objetivo
       22  
       20 -<Uma frase. O que passa a ser verdade quando isto existir.>   
       23 +Dado o conjunto de despesas de um colaborador numa competência
          +, o sistema informa,                                          
       24 +para cada despesa, o valor reembolsável e a justificativa da d
          +ecisão, aplicando                                             
       25 +sempre as mesmas interpretações da política, todas registradas
          + nesta spec.                                                  
       26  
       27  ## 3. Fora de escopo
       28  
       24 -<Lista explícita. Este bloco vale ponto: ele impede o agente d
          -e inventar feature                                            
       25 -e impede você de mover a trave depois.>                       
       29 +- Não detecta automaticamente refeição com cliente ou despesa 
          +de representação.                                             
       30 +  A descrição da despesa é texto livre e não é interpretada. I
          +sso é orientação para                                         
       31 +  o revisor humano (AMB-022).                                 
       32 +- Não decide o resultado final de itens em revisão humana. O s
          +istema só os sinaliza                                         
       33 +  e informa o motivo (RN-010).                                
       34 +- Não verifica, entre as despesas, se uma duplicata sinalizada
          + é legítima, nem se o                                         
       35 +  café da manhã já estava incluído na nota de hospedagem, nem 
          +se uma mesma nota de                                          
       36 +  hospedagem foi anexada a mais de uma diária. Essa checagem é
          + humana (AMB-007,                                             
       37 +  AMB-023, AMB-024).                                          
       38 +- Não processa estornos nem devoluções. Eles seguem processo p
          +róprio, fora deste                                            
       39 +  sistema (AMB-017).                                          
       40 +- Não trata de horas extras, plantão, feriado ou fim de semana
          +. Esses dias recebem                                          
       41 +  os mesmos limites de qualquer outro dia (AMB-019).          
       42 +- Não extrai número de diárias, motivo da despesa ou indício d
          +e viagem do texto                                             
       43 +  livre das despesas (AMB-015, AMB-014).                      
       44 +- Não define comportamento quando `competencia` e `inicio`/`fi
          +m` divergem (AMB-011).                                        
       45 +- Não efetua pagamento, não converte moedas (todos os valores 
          +são em reais) e não                                           
       46 +  processa mais de um colaborador por arquivo de entrada.     
       47  
       27 -- Não faz `<...>`                                             
       28 -- Não faz `<...>`                                             
       29 -                                                              
       48  ## 4. Entrada e saída
       49  
       50  **Entrada:** conforme `exemplos/despesas-exemplo.json`. Campos
            e significado:
       51  
       52  | Campo | Tipo | Significado | Obrigatório |
       53  |---|---|---|---|
       36 -| | | | |                                                     
       54 +| `colaborador.id` | texto | Identificador do colaborador | Si
          +m |                                                           
       55 +| `colaborador.nome` | texto | Nome, apenas informativo | Não 
          +|                                                             
       56 +| `colaborador.centro_custo` | texto | Centro de custo, apenas
          + informativo | Não |                                          
       57 +| `periodo.competencia` | texto `AAAA-MM` | Mês de competência
          +. É a referência para a regra de período (AMB-011) | Sim |    
       58 +| `periodo.inicio`, `periodo.fim` | data | Limites informados.
          + Não são usados pelas regras (AMB-011) | Não |                
       59 +| `despesas` | lista | Despesas do colaborador na competência.
          + A ordem da lista é a "ordem do arquivo" (AMB-002) | Sim |    
       60 +| `despesas[].id` | texto | Identificador único da despesa | S
          +im |                                                          
       61 +| `despesas[].data` | data | Data em que a despesa ocorreu | S
          +im |                                                          
       62 +| `despesas[].categoria` | texto | Categoria informada (RN-001
          +) | Sim |                                                     
       63 +| `despesas[].descricao` | texto | Descrição livre, apenas inf
          +ormativa | Não |                                              
       64 +| `despesas[].fornecedor` | texto | Fornecedor da despesa | Si
          +m |                                                           
       65 +| `despesas[].valor` | número | Valor em reais. Pode ter mais 
          +de 2 casas decimais (RN-009) | Sim |                          
       66 +| `despesas[].tem_nota_fiscal` | verdadeiro/falso | Se há nota
          + fiscal | Sim |                                               
       67  
       38 -**Saída:** definida por mim. Estrutura e significado de cada c
          -ampo:                                                         
       68 +**Saída:** um resultado por arquivo de entrada, com um item po
          +r despesa.                                                    
       69  
       70  | Campo | Tipo | Significado |
       71  |---|---|---|
       42 -| | | |                                                       
       72 +| `colaborador_id` | texto | Copiado da entrada |             
       73 +| `competencia` | texto | Copiada da entrada |                
       74 +| `viagem` | verdadeiro/falso | Se a competência foi caracteri
          +zada como viagem (RN-006) |                                   
       75 +| `total_reembolsavel` | número, 2 casas | Soma dos `valor_ree
          +mbolsavel` dos itens `aprovado` (RN-011) |                    
       76 +| `resumo` | objeto | Quantidade de itens por status: `aprovad
          +o`, `recusado`, `em_revisao` |                                
       77 +| `itens` | lista | Um elemento por despesa da entrada, na mes
          +ma ordem |                                                    
       78 +| `itens[].id` | texto | Id da despesa |                      
       79 +| `itens[].categoria` | texto | Categoria normalizada (RN-001)
          +, ou a informada se não reconhecida |                         
       80 +| `itens[].status` | texto | `aprovado` (inclui pagamento parc
          +ial), `recusado` (final, valor 0) ou `em_revisao` (valor 0, ag
          +uarda revisor humano) |                                       
       81 +| `itens[].valor_solicitado` | número, 2 casas | Valor da desp
          +esa depois do arredondamento (RN-009) |                       
       82 +| `itens[].valor_reembolsavel` | número, 2 casas | Valor a ree
          +mbolsar. É 0,00 quando o status é `recusado` ou `em_revisao` |
       83 +| `itens[].justificativa` | texto | Explicação em linguagem na
          +tural da decisão |                                            
       84 +| `itens[].regras` | lista de texto | Ids das regras (RN-xxx) 
          +que determinaram o resultado |                                
       85 +| `itens[].alertas` | lista de texto | Advertências para o rev
          +isor humano (por exemplo, possível duplicata). Pode ser vazia 
          +|                                                             
       86  
       44 -<Cole um exemplo de saída para uma entrada pequena. Vale mais 
          -que três parágrafos.>                                         
       87 +Exemplo de saída para um subconjunto do exemplo oficial (d-001
          +, d-002, d-004 e                                              
       88 +d-010, nesta ordem):                                          
       89  
       90 +```                                                           
       91 +{                                                             
       92 +  "colaborador_id": "c-0417",                                 
       93 +  "competencia": "2026-07",                                   
       94 +  "viagem": true,                                             
       95 +  "total_reembolsavel": 340.00,                               
       96 +  "resumo": { "aprovado": 3, "recusado": 1, "em_revisao": 0 },
       97 +  "itens": [                                                  
       98 +    { "id": "d-001", "categoria": "alimentacao", "status": "ap
          +rovado",                                                      
       99 +      "valor_solicitado": 72.50, "valor_reembolsavel": 72.50, 
      100 +      "justificativa": "Dentro do saldo diário de alimentação 
          +(limite 90,00 em viagem).",                                   
      101 +      "regras": ["RN-001", "RN-006", "RN-007"], "alertas": [] 
          +},                                                            
      102 +    { "id": "d-002", "categoria": "alimentacao", "status": "ap
          +rovado",                                                      
      103 +      "valor_solicitado": 38.00, "valor_reembolsavel": 17.50, 
      104 +      "justificativa": "Restava 17,50 do saldo diário de 90,00
          +; excedente cortado.",                                        
      105 +      "regras": ["RN-001", "RN-006", "RN-007"], "alertas": [] 
          +},                                                            
      106 +    { "id": "d-004", "categoria": "transporte_urbano", "status
          +": "recusado",                                                
      107 +      "valor_solicitado": 100.01, "valor_reembolsavel": 0.00, 
      108 +      "justificativa": "Valor acima de 100,00 sem nota fiscal.
          +",                                                            
      109 +      "regras": ["RN-004"], "alertas": [] },                  
      110 +    { "id": "d-010", "categoria": "hospedagem", "status": "apr
          +ovado",                                                       
      111 +      "valor_solicitado": 480.00, "valor_reembolsavel": 250.00
          +,                                                             
      112 +      "justificativa": "Lançamento conta como uma diária; limi
          +te de 250,00; excedente cortado.",                            
      113 +      "regras": ["RN-001", "RN-008"], "alertas": [] }         
      114 +  ]                                                           
      115 +}                                                             
      116 +```                                                           
      117 +                                                              
      118  ## 5. Regras de negócio
      119  
      120  Cada regra recebe um ID (`RN-001`, ...). As tasks vão referenc
           iar esses IDs.
      121 +Salvo indicação contrária, os exemplos de aceite usam o arquiv
          +o                                                             
      122 +`exemplos/despesas-exemplo.json` completo. Onde o exemplo diz 
          +"sozinho" ou "sem                                             
      123 +d-0xx", considera-se o mesmo arquivo com apenas aquelas despes
          +as, mantida a ordem.                                          
      124  
       50 -### RN-001 — <nome da regra>                                  
      125 +### RN-001 — Categorias aceitas e normalização                
      126  
       52 -**Regra:** <enunciado sem ambiguidade>                        
       53 -**Origem:** política do RH, item `<n>`                        
       54 -**Aceite:** <como verificar que está implementada — normalment
          -e um caso concreto com números>                               
      127 +**Regra:** só são reembolsáveis `alimentacao`, `transporte_urb
          +ano` e `hospedagem`.                                          
      128 +Antes de comparar, a categoria informada é normalizada: ignora
          +-se caixa, acentos e                                          
      129 +espaços nas pontas, e espaço ou hífen entre palavras equivale 
          +a sublinhado. Se,                                             
      130 +após a normalização, a categoria for igual a uma das três, ela
          + é usada com esse                                             
      131 +nome. Qualquer outra categoria torna a despesa `recusado`, com
          + alerta de                                                    
      132 +encaminhamento para análise humana.                           
      133 +**Origem:** política do RH, item 9                            
      134 +**Aceite:** d-014 (`ALIMENTACAO`) é tratada como `alimentacao`
          + e fica `aprovado`                                            
      135 +com 61,00. d-005 (`coworking`) fica `recusado` com 0,00.      
      136  
       56 -### RN-002 — ...                                              
      137 +### RN-002 — Valor negativo                                   
      138  
      139 +**Regra:** despesa com valor negativo é dado inválido e fica `
          +recusado`. Os                                                 
      140 +demais itens do arquivo são processados normalmente.          
      141 +**Origem:** decisão desta spec (a política não trata valores n
          +egativos)                                                     
      142 +**Aceite:** d-009 (−45,00) fica `recusado` com 0,00 e não alte
          +ra o saldo nem o                                              
      143 +total de nenhuma outra despesa.                               
      144 +                                                              
      145 +### RN-003 — Período de competência                           
      146 +                                                              
      147 +**Regra:** a data da despesa deve cair no mês indicado em `com
          +petencia`. A despesa                                          
      148 +fora do mês fica `em_revisao`, e o revisor humano decide a apr
          +ovação ou rejeição.                                           
      149 +**Origem:** política do RH, item 7                            
      150 +**Aceite:** d-008 (2026-04-15, competência 2026-07) fica `em_r
          +evisao` com 0,00.                                             
      151 +d-014 (2026-07-31) está dentro do período.                    
      152 +                                                              
      153 +### RN-004 — Nota fiscal obrigatória                          
      154 +                                                              
      155 +**Regra:** despesa com valor **acima** de R$ 100,00 e sem nota
          + fiscal fica                                                  
      156 +`recusado` com valor 0,00, mesmo que o limite permitisse pagar
          + algo. Até R$ 100,00                                          
      157 +inclusive, a nota é dispensada. A verificação usa o valor de c
          +ada despesa                                                   
      158 +individualmente, antes de qualquer corte por limite, e o limia
          +r de R$ 100,00 não                                            
      159 +muda em viagem.                                               
      160 +**Origem:** política do RH, item 5                            
      161 +**Aceite:** d-003 (100,00, sem nota) não é recusada por falta 
          +de nota. d-004                                                
      162 +(100,01, sem nota) fica `recusado`. d-013 (690,00, sem nota) f
          +ica `recusado`.                                               
      163 +                                                              
      164 +### RN-005 — Duplicatas                                       
      165 +                                                              
      166 +**Regra:** duas ou mais despesas que chegam a este passo (ver 
          +seção 8) com a mesma                                          
      167 +data, a mesma categoria normalizada, o mesmo fornecedor e o me
          +smo valor formam um                                           
      168 +grupo de possíveis duplicatas. Todos os itens do grupo ficam `
          +em_revisao`, com                                              
      169 +alerta que cita os ids do grupo.                              
      170 +**Origem:** política do RH, item 8                            
      171 +**Aceite:** d-006 e d-007 ficam `em_revisao`, 0,00 cada, com a
          +lerta citando os dois                                         
      172 +ids. d-001 e d-002 (fornecedores diferentes) e d-003 e d-004 (
          +valores                                                       
      173 +diferentes) não formam grupo.                                 
      174 +                                                              
      175 +### RN-006 — Viagem                                           
      176 +                                                              
      177 +**Regra:** a competência é de viagem se existir ao menos uma d
          +espesa de hospedagem                                          
      178 +que chegue ao passo de viagem (seção 8) sem ter sido recusada 
          +nem enviada à                                                 
      179 +revisão. Quando é viagem, vale para toda a competência: o limi
          +te diário de                                                  
      180 +alimentação passa de 60,00 para 90,00 e o de transporte urbano
          + de 80,00 para                                                
      181 +120,00. O limite de hospedagem (250,00) e o limiar de nota fis
          +cal (100,00) não                                              
      182 +mudam.                                                        
      183 +**Origem:** política do RH, item 6                            
      184 +**Aceite:** no arquivo completo, `viagem` é verdadeiro porque 
          +d-010 é aprovada.                                             
      185 +Sem d-010 e sem d-013, `viagem` é falso, e d-014 sozinha paga 
          +60,00. Com d-013 como                                         
      186 +única hospedagem, `viagem` é falso, pois d-013 é recusada por 
          +RN-004.                                                       
      187 +                                                              
      188 +### RN-007 — Limite diário de alimentação e transporte urbano 
      189 +                                                              
      190 +**Regra:** o limite vale para a soma da categoria no dia (data
          + da despesa). No                                              
      191 +início de cada dia e categoria, o saldo é o limite aplicável (
          +60,00 e 80,00, ou                                             
      192 +90,00 e 120,00 em viagem, conforme RN-006). As despesas que ch
          +egam a este passo                                             
      193 +são processadas na ordem do arquivo. Cada uma recebe o menor v
          +alor entre o                                                  
      194 +solicitado e o saldo restante, e esse valor é abatido do saldo
          +. Valor igual ao                                              
      195 +limite é pago integralmente. O excedente é cortado. Se o saldo
          + restante for 0,                                              
      196 +o item fica `recusado` com 0,00. Fim de semana, feriado e plan
          +tão não alteram o                                             
      197 +limite. Café da manhã de hotel é alimentação como qualquer out
          +ra.                                                           
      198 +**Origem:** política do RH, itens 1, 2 e 4                    
      199 +**Aceite:** no arquivo completo (viagem, limites 90,00 e 120,0
          +0):                                                           
      200 +d-001 paga 72,50 e d-002 paga 17,50, num total de 90,00. d-003
          + paga 100,00.                                                 
      201 +d-011 paga 33,33. d-012 paga 47,20. d-014 paga 61,00.         
      202 +Sem viagem (d-001 e d-002 sozinhas): d-001 paga 60,00 e d-002 
          +fica `recusado`                                               
      203 +com 0,00. d-003 sozinha paga 80,00. d-014 sozinha paga 60,00. 
      204 +                                                              
      205 +### RN-008 — Limite de hospedagem por diária                  
      206 +                                                              
      207 +**Regra:** cada despesa de hospedagem equivale a uma diária, e
          + o reembolsável é o                                           
      208 +menor valor entre o solicitado e 250,00. O número de diárias o
          +u noites que                                                  
      209 +aparecer na descrição é ignorado. Diárias devem ser lançadas u
          +ma a uma, e a mesma                                           
      210 +nota pode ser anexada a várias.                               
      211 +**Origem:** política do RH, item 3                            
      212 +**Aceite:** d-010 (480,00, "2 diarias") paga 250,00, com 230,0
          +0 cortados. d-013                                             
      213 +("3 noites") seria limitada a 250,00 por esta regra, mas é rec
          +usada antes por                                               
      214 +RN-004.                                                       
      215 +                                                              
      216 +### RN-009 — Arredondamento                                   
      217 +                                                              
      218 +**Regra:** todo valor de entrada é arredondado para 2 casas de
          +cimais, com                                                   
      219 +arredondamento bancário (empate vai para o dígito par), logo n
          +a leitura, antes de                                           
      220 +qualquer regra ou soma.                                       
      221 +**Origem:** decisão desta spec (a política não trata centavos)
      222 +**Aceite:** d-011 (33,333) tem `valor_solicitado` 33,33. Em ca
          +sos hipotéticos de                                            
      223 +empate, 10,125 vira 10,12 e 10,135 vira 10,14.                
      224 +                                                              
      225 +### RN-010 — Itens em revisão humana                          
      226 +                                                              
      227 +**Regra:** item `em_revisao` tem `valor_reembolsavel` 0,00, nã
          +o consome saldo                                               
      228 +diário, não conta como evidência de viagem e não entra em `tot
          +al_reembolsavel`. A                                           
      229 +saída informa o motivo e os alertas. A decisão final é do revi
          +sor humano, fora do                                           
      230 +sistema.                                                      
      231 +**Origem:** decisão desta spec (política, itens 7 e 8)        
      232 +**Aceite:** d-006, d-007 e d-008 ficam `em_revisao` com 0,00, 
          +não alteram o saldo                                           
      233 +das demais despesas de alimentação e não entram no total.     
      234 +                                                              
      235 +### RN-011 — Totalização e justificativa                      
      236 +                                                              
      237 +**Regra:** `total_reembolsavel` é a soma dos `valor_reembolsav
          +el` de todos os itens                                         
      238 +`aprovado`. Cada despesa da entrada aparece uma única vez na s
          +aída, na ordem da                                             
      239 +entrada, com `status`, `justificativa` e `regras`.            
      240 +**Origem:** objetivo desta spec                               
      241 +**Aceite:** no arquivo completo, `total_reembolsavel` é 581,53
          +. `resumo` mostra 7                                           
      242 +`aprovado`, 4 `recusado` e 3 `em_revisao`, e a saída tem 14 it
          +ens.                                                          
      243 +                                                              
      244  ---
      245  
      246  ## 6. Ambiguidades identificadas e decisões
     ...
      249  > Uma ambiguidade que você resolveu no código sem registrar aq
           ui conta como
      250  > não resolvida.
      251  
       66 -### AMB-001 — <o que a política deixou em aberto>             
      252 +### AMB-001 — Limite "por dia": por dia ou por despesa        
      253  
       68 -**Texto original do RH:** "<citação literal>"                 
       69 -**O que não está claro:** <as duas ou mais leituras possíveis>
       70 -**Decisão:** <o que o sistema faz>                            
       71 -**Justificativa:** <por quê — uma linha; critério de negócio, 
          -não de conveniência técnica>                                  
       72 -**Regra afetada:** RN-00X                                     
      254 +**Texto original do RH:** "Alimentação tem limite de R$ 60 por
          + dia." / "Transporte urbano tem limite de R$ 80 por dia."     
      255 +**O que não está claro:** o teto vale para a soma da categoria
          + no dia, ou para cada despesa individualmente.                
      256 +**Decisão:** o teto vale para a soma da categoria no dia. Os p
          +ares d-001/d-002 e d-006/d-007 são limitados ao teto diário, e
          + o par d-003/d-004 também.                                    
      257 +**Justificativa:** a política não fala em limite por refeição,
          + mas em limite diário.                                        
      258 +**Regra afetada:** RN-007                                     
      259  
       74 -### AMB-002 — ...                                             
      260 +### AMB-002 — Como distribuir o corte dentro do dia           
      261  
       76 -<A política tem no mínimo oito. Se você achou menos, releia   
       77 -`exemplos/despesas-exemplo.json` — cada item daquele arquivo e
          -xiste por um motivo.>                                         
      262 +**Texto original do RH:** "Despesas acima do limite são reembo
          +lsadas parcialmente."                                         
      263 +**O que não está claro:** quando várias despesas excedem o tet
          +o do dia, qual delas sofre o corte: a primeira, a última, ou u
          +m rateio.                                                     
      264 +**Decisão:** vale a ordem do arquivo. O primeiro item consome 
          +o saldo e os seguintes recebem o que restar.                  
      265 +**Justificativa:** o limite diário é um saldo a consumir, que 
          +é aproveitado pelo lançamento seguinte.                       
      266 +**Regra afetada:** RN-007                                     
      267  
      268 +### AMB-003 — "Reembolsadas parcialmente"                     
      269 +                                                              
      270 +**Texto original do RH:** "Despesas acima do limite são reembo
          +lsadas parcialmente."                                         
      271 +**O que não está claro:** paga-se o limite e corta-se o excede
          +nte, ou recusa-se o item inteiro.                             
      272 +**Decisão:** paga o limite e corta o excedente.               
      273 +**Justificativa:** o valor do limite representa o valor a que 
          +o colaborador tem direito.                                    
      274 +**Regra afetada:** RN-007, RN-008                             
      275 +                                                              
      276 +### AMB-004 — Hospedagem "por diária"                         
      277 +                                                              
      278 +**Texto original do RH:** "Hospedagem tem limite de R$ 250 por
          + diária."                                                     
      279 +**O que não está claro:** o limite vale por diária (com o núme
          +ro de diárias do lançamento) ou por lançamento.               
      280 +**Decisão:** o limite de 250,00 vale por diária, e a diária co
          +nta o número de noites. Entretanto, prevalece a AMB-015: cada 
          +lançamento é uma única diária, e a quantidade escrita na descr
          +ição é ignorada. Assim, d-010 (480,00) paga 250,00.           
      281 +**Justificativa:** a diária de hospedagem conta noites, e o la
          +nçamento individual por noite (AMB-015, AMB-024) evita inferir
          + a quantidade do texto.                                       
      282 +**Regra afetada:** RN-008                                     
      283 +                                                              
      284 +### AMB-005 — Nota fiscal: a qual valor se aplica             
      285 +                                                              
      286 +**Texto original do RH:** "Nota fiscal é obrigatória acima de 
          +R$ 100."                                                      
      287 +**O que não está claro:** o limite vale para o valor de cada d
          +espesa, para a soma do dia, ou para o valor depois do corte do
          + limite.                                                      
      288 +**Decisão:** vale para o valor de cada despesa individualmente
          +.                                                             
      289 +**Justificativa:** despesas acima de R$ 100 exigem evidência p
          +ara aprovação, mesmo que o limite de diária seja menor que ess
          +e valor.                                                      
      290 +**Regra afetada:** RN-004                                     
      291 +                                                              
      292 +### AMB-006 — Quais limites a viagem amplia                   
      293 +                                                              
      294 +**Texto original do RH:** "Colaborador em viagem tem limites a
          +mpliados em 50%."                                             
      295 +**O que não está claro:** a ampliação vale para os três tetos 
          +diários, só para alimentação e transporte, ou também para o li
          +miar de R$ 100 da nota fiscal.                                
      296 +**Decisão:** só alimentação e transporte urbano. A hospedagem 
          +e o limiar da nota ficam como estão.                          
      297 +**Justificativa:** a hospedagem é a evidência da ampliação e n
          +ão muda o seu próprio limite nem a exigência de nota, que cont
          +inua em R$ 100.                                               
      298 +**Regra afetada:** RN-006                                     
      299 +                                                              
      300 +### AMB-007 — O que é duplicata                               
      301 +                                                              
      302 +**Texto original do RH:** "Duplicatas devem ser tratadas."    
      303 +**O que não está claro:** quais campos precisam coincidir para
          + duas despesas serem duplicatas.                              
      304 +**Decisão:** data, categoria, fornecedor e valor. A duplicata 
          +é uma advertência, e as informações são checadas depois, em aç
          +ão de validação fora do escopo.                               
      305 +**Justificativa:** pode haver mais de um consumo igual, no mes
          +mo dia, no mesmo fornecedor e categoria.                      
      306 +**Regra afetada:** RN-005                                     
      307 +                                                              
      308 +### AMB-008 — Como tratar as duplicatas                       
      309 +                                                              
      310 +**Texto original do RH:** "Duplicatas devem ser tratadas."    
      311 +**O que não está claro:** o tratamento pode ser descartar a se
          +gunda, recusar as duas, pagar as duas com sinalização, ou revi
          +são humana.                                                   
      312 +**Decisão:** todas as duplicatas, ou seja, todos os itens do g
          +rupo, vão para revisão humana, com valor 0,00 até a decisão.  
      313 +**Justificativa:** todos os itens identificados como duplicata
          + devem ser checados.                                          
      314 +**Regra afetada:** RN-005, RN-010                             
      315 +                                                              
      316 +### AMB-009 — Nota fiscal com exatamente R$ 100,00            
      317 +                                                              
      318 +**Texto original do RH:** "Nota fiscal é obrigatória acima de 
          +R$ 100."                                                      
      319 +**O que não está claro:** a despesa de exatamente R$ 100,00 pr
          +ecisa de nota ou não.                                         
      320 +**Decisão:** "acima" é estrito. d-003 (100,00) não precisa de 
          +nota, e d-004 (100,01) precisa.                               
      321 +**Justificativa:** até R$ 100 se entende que é tolerado sem no
          +ta.                                                           
      322 +**Regra afetada:** RN-004                                     
      323 +                                                              
      324 +### AMB-010 — Valor exatamente igual ao limite                
      325 +                                                              
      326 +**Texto original do RH:** "Despesas acima do limite são reembo
          +lsadas parcialmente."                                         
      327 +**O que não está claro:** o valor igual ao limite é pago integ
          +ralmente ou já conta como excesso.                            
      328 +**Decisão:** igual ao limite está dentro e é pago integralment
          +e.                                                            
      329 +**Justificativa:** o pagamento parcial só vale acima do limite
          +, conforme o saldo ainda existente na categoria.              
      330 +**Regra afetada:** RN-007, RN-008                             
      331 +                                                              
      332 +### AMB-011 — Extremos do período e fonte da verdade          
      333 +                                                              
      334 +**Texto original do RH:** "Despesas devem ser lançadas dentro 
          +do período de competência."                                   
      335 +**O que não está claro:** a referência do período são os campo
          +s `inicio`/`fim` (inclusivos ou exclusivos) ou o campo `compet
          +encia`.                                                       
      336 +**Decisão:** a fonte é o campo `competencia` (por exemplo, "20
          +26-07") e não `inicio`/`fim`. Se os dois divergirem, não há re
          +gra definida (seção 10).                                      
      337 +**Justificativa:** o reembolso está atrelado à competência inf
          +ormada.                                                       
      338 +**Regra afetada:** RN-003                                     
      339 +                                                              
      340 +### AMB-012 — Arredondamento                                  
      341 +                                                              
      342 +**Texto original do RH:** a política não diz nada sobre centav
          +os.                                                           
      343 +**O que não está claro:** como tratar valores com mais de 2 ca
          +sas decimais (d-011 = 33,333): arredondar para cima, truncar, 
          +arredondamento bancário ou rejeitar.                          
      344 +**Decisão:** arredondamento bancário, com 2 casas decimais, na
          + leitura de cada valor.                                       
      345 +**Justificativa:** segue o modelo bancário e evita divergência
          + entre saldos.                                                
      346 +**Regra afetada:** RN-009                                     
      347 +                                                              
      348 +### AMB-013 — Rótulo da categoria em maiúsculas               
      349 +                                                              
      350 +**Texto original do RH:** "Categorias fora da política não são
          + reembolsáveis."                                              
      351 +**O que não está claro:** `ALIMENTACAO` (d-014) é alimentação,
          + está fora da política ou é erro de dado.                     
      352 +**Decisão:** a comparação ignora maiúsculas e minúsculas, entã
          +o d-014 é alimentação e entra no limite. Variações de acento e
          + espaço também são normalizadas quando for possível inferir um
          +a categoria aceita. Os demais casos são rejeitados e encaminha
          +dos para análise humana.                                      
      353 +**Justificativa:** categorias não identificadas devem ser reje
          +itadas e encaminhadas à análise humana.                       
      354 +**Regra afetada:** RN-001                                     
      355 +                                                              
      356 +### AMB-014 — O que é "em viagem"                             
      357 +                                                              
      358 +**Texto original do RH:** "Colaborador em viagem tem limites a
          +mpliados em 50%."                                             
      359 +**O que não está claro:** a entrada não tem campo de viagem. A
          + viagem pode ser inferida pelo dia, pelo período ou por toda a
          + competência.                                                 
      360 +**Decisão:** toda a competência é viagem se houver qualquer ho
          +spedagem aprovada. Uma hospedagem recusada ou em revisão não c
          +onta.                                                         
      361 +**Justificativa:** sem hospedagem trata-se de deslocamento cur
          +to, que não caracteriza viagem.                               
      362 +**Regra afetada:** RN-006                                     
      363 +                                                              
      364 +### AMB-015 — Número de diárias só no texto livre             
      365 +                                                              
      366 +**Texto original do RH:** "Hospedagem tem limite de R$ 250 por
          + diária."                                                     
      367 +**O que não está claro:** o número de diárias só existe na des
          +crição ("2 diarias", "3 noites"). Pode ser extraído do texto, 
          +assumido como 1 ou calculado.                                 
      368 +**Decisão:** cada diária deve ser lançada individualmente. Se 
          +a descrição de um lançamento indicar mais de uma, ele conta co
          +mo uma única diária.                                          
      369 +**Justificativa:** a quantidade escrita no texto não é confiáv
          +el, então o lançamento individual evita depender dela.        
      370 +**Regra afetada:** RN-008                                     
      371 +                                                              
      372 +### AMB-016 — Data da despesa × data do lançamento            
      373 +                                                              
      374 +**Texto original do RH:** "Despesas devem ser lançadas dentro 
          +do período de competência."                                   
      375 +**O que não está claro:** a entrada só tem a data da despesa, 
          +sem data de lançamento. Uma despesa antiga lançada com atraso 
          +(d-008) pode ser recusada, aceita ou depender de exceção.     
      376 +**Decisão:** a despesa fora do período é aceita com exceção: v
          +ai para revisão humana (a comparação usa a data da despesa).  
      377 +**Justificativa:** a aprovação ou rejeição fica a cargo do rev
          +isor humano.                                                  
      378 +**Regra afetada:** RN-003, RN-010                             
      379 +                                                              
      380 +### AMB-017 — Estorno                                         
      381 +                                                              
      382 +**Texto original do RH:** a política não trata valores negativ
          +os.                                                           
      383 +**O que não está claro:** o estorno (d-009, −45,00) pode ser i
          +gnorado, abatido do total, abatido do dia ou rejeitado.       
      384 +**Decisão:** é dado inválido e é rejeitado (somente o item).  
      385 +**Justificativa:** o processo é de reembolso e recebe apenas v
          +alores reembolsáveis. Devoluções são lançadas em processo espe
          +cífico, fora deste escopo.                                    
      386 +**Regra afetada:** RN-002                                     
      387 +                                                              
      388 +### AMB-018 — Categoria sem mapeamento                        
      389 +                                                              
      390 +**Texto original do RH:** "Categorias fora da política não são
          + reembolsáveis."                                              
      391 +**O que não está claro:** a política só nomeia três categorias
          +. `coworking` (d-005) poderia ser tratada como hospedagem ou c
          +omo categoria desconhecida.                                   
      392 +**Decisão:** a lista fechada é alimentação, transporte urbano 
          +e hospedagem. Coworking está fora e vale 0,00.                
      393 +**Justificativa:** somente categorias reembolsáveis são válida
          +s, e as demais são rejeitadas.                                
      394 +**Regra afetada:** RN-001                                     
      395 +                                                              
      396 +### AMB-019 — Fim de semana e plantão                         
      397 +                                                              
      398 +**Texto original do RH:** "Alimentação tem limite de R$ 60 por
          + dia."                                                        
      399 +**O que não está claro:** "dia" poderia significar dia útil, o
          +u fim de semana e plantão (d-012, sábado) poderiam ter regra p
          +rópria.                                                       
      400 +**Decisão:** é um dia como qualquer outro, e vale o limite nor
          +mal de acordo com as demais regras.                           
      401 +**Justificativa:** fins de semana, feriados e plantões são tra
          +tados externamente como horas extras trabalhadas, sem mudar os
          + limites de reembolso.                                        
      402 +**Regra afetada:** RN-007                                     
      403 +                                                              
      404 +### AMB-021 — Precedência: nota fiscal × limite               
      405 +                                                              
      406 +**Texto original do RH:** "Despesas acima do limite são reembo
          +lsadas parcialmente." / "Nota fiscal é obrigatória acima de R$
          + 100."                                                        
      407 +**O que não está claro:** a despesa sem nota deve ser recusada
          + por inteiro ou paga até o valor que dispensaria a nota.      
      408 +**Decisão:** sem nota, recusa total (zera o item), mesmo que o
          + limite permitisse pagar algo.                                
      409 +**Justificativa:** a nota é obrigatória para aceitação.       
      410 +**Regra afetada:** RN-004, seção 8                            
      411 +                                                              
      412 +### AMB-022 — Despesa com terceiros                           
      413 +                                                              
      414 +**Texto original do RH:** "Categorias fora da política não são
          + reembolsáveis."                                              
      415 +**O que não está claro:** refeição com cliente (d-001, "Almoco
          + com cliente") poderia ser representação, uma categoria fora d
          +a política, ou dividir o limite por pessoa.                   
      416 +**Decisão:** não há detecção automática. A descrição é texto l
          +ivre e não é interpretada; a decisão vira orientação para a re
          +visão humana. d-001 fica `aprovado` e entra no limite diário n
          +ormalmente.                                                   
      417 +**Justificativa:** o reembolso é exclusivo do colaborador, mas
          + a entrada não traz dado confiável para identificar o terceiro
          +.                                                             
      418 +**Regra afetada:** nenhuma (ver seção 3)                      
      419 +                                                              
      420 +### AMB-023 — Sobreposição entre categorias na mesma estadia  
      421 +                                                              
      422 +**Texto original do RH:** "Alimentação tem limite de R$ 60 por
          + dia." / "Hospedagem tem limite de R$ 250 por diária."        
      423 +**O que não está claro:** o café da manhã do hotel (d-011) pod
          +e estar incluído na diária (d-010).                           
      424 +**Decisão:** são despesas independentes, e o café entra no lim
          +ite diário de alimentação, desde que não faça parte da nota de
          + hospedagem. Essa checagem é feita na validação humana, fora d
          +o escopo.                                                     
      425 +**Justificativa:** o café pode não estar incluído na diária de
          + hospedagem.                                                  
      426 +**Regra afetada:** RN-007                                     
      427 +                                                              
      428 +### AMB-024 — Despesa de vários dias, com data única          
      429 +                                                              
      430 +**Texto original do RH:** "Hospedagem tem limite de R$ 250 por
          + diária."                                                     
      431 +**O que não está claro:** uma hospedagem de várias noites tem 
          +uma data só, o que afeta a atribuição de dias.                
      432 +**Decisão:** cada diária deve ser incluída de forma independen
          +te, e a mesma nota pode ser anexada a todas. A checagem fica c
          +om a validação humana.                                        
      433 +**Justificativa:** padroniza e facilita o processo.           
      434 +**Regra afetada:** RN-008                                     
      435 +                                                              
      436  ---
      437  
      438  ## 7. Casos de borda
      439  
      440  | Caso | Entrada | Comportamento esperado | Regra |
      441  |---|---|---|---|
       85 -| | | | |                                                     
      442 +| Nota no limiar exato | d-003 (100,00, sem nota) | Não exige 
          +nota. `aprovado` com 100,00 | RN-004 |                        
      443 +| Um centavo acima do limiar | d-004 (100,01, sem nota) | `rec
          +usado`, 0,00 | RN-004 |                                       
      444 +| Sem nota, mesmo dentro do limite | d-013 (690,00, sem nota) 
          +| `recusado`, 0,00 | RN-004 |                                 
      445 +| Duplicata | d-006 e d-007 | Ambas `em_revisao`, 0,00, alerta
          + com os dois ids | RN-005 |                                   
      446 +| Fora do período | d-008 (2026-04-15) | `em_revisao`, 0,00 | 
          +RN-003 |                                                      
      447 +| Último dia do mês | d-014 (2026-07-31) | Dentro do período |
          + RN-003 |                                                     
      448 +| Estorno | d-009 (−45,00) | `recusado`, 0,00. Demais itens in
          +tactos | RN-002 |                                             
      449 +| Categoria fora da política | d-005 (coworking) | `recusado`,
          + 0,00 | RN-001 |                                              
      450 +| Categoria em maiúsculas | d-014 (`ALIMENTACAO`, 61,00) | Nor
          +malizada. Paga 61,00 em viagem e 60,00 sem viagem | RN-001, RN
          +-007 |                                                        
      451 +| Mais de 2 casas decimais | d-011 (33,333) | `valor_solicitad
          +o` 33,33 | RN-009 |                                           
      452 +| Diária múltipla no texto | d-010 (480,00, "2 diarias") | Con
          +ta como 1 diária, paga 250,00 | RN-008 |                      
      453 +| Dois itens no mesmo dia | d-001 e d-002 | Em viagem: 72,50 e
          + 17,50. Sem viagem: 60,00 e 0,00 (`recusado`) | RN-007 |      
      454 +| Dia de fim de semana | d-012 (sábado, 47,20) | Sem tratament
          +o especial. `aprovado` com 47,20 | RN-007 |                   
      455 +| Refeição com cliente | d-001 | Sem detecção. `aprovado` norm
          +almente | seção 3 |                                           
      456 +| Valor igual ao limite (hipotético) | Alimentação 60,00 sem v
          +iagem | Paga 60,00 integralmente | RN-007 |                   
      457  
      458  ## 8. Ordem de aplicação das regras
      459  
       89 -<Quando duas regras incidem sobre a mesma despesa, qual vale p
          -rimeiro? A ordem                                              
       90 -muda o resultado. Declarar isso separa spec boa de spec média.
          ->                                                             
      460 +A ordem muda o resultado: um item recusado ou em revisão não c
          +onsome saldo, não                                             
      461 +participa da comparação de duplicatas e não conta como evidênc
          +ia de viagem.                                                 
      462  
      463 +1. **Arredondamento** do valor (RN-009).                      
      464 +2. **Valor negativo** (RN-002): `recusado`.                   
      465 +3. **Categoria** (RN-001): normaliza e, se não reconhecida, `r
          +ecusado`.                                                     
      466 +4. **Período** (RN-003): fora da competência, `em_revisao`.   
      467 +5. **Nota fiscal** (RN-004): acima de 100,00 sem nota, `recusa
          +do`.                                                          
      468 +6. **Duplicata** (RN-005): grupos entre os itens ainda ativos,
          + `em_revisao`.                                                
      469 +7. **Viagem** (RN-006): decidida com as hospedagens ainda ativ
          +as.                                                           
      470 +8. **Limite** (RN-007 e RN-008): alimentação e transporte por 
          +dia, em ordem do                                              
      471 +   arquivo; hospedagem por lançamento.                        
      472 +9. **Totalização** (RN-011).                                  
      473 +                                                              
      474 +Consequências: duas despesas iguais e sem nota acima de 100,00
          + são ambas                                                    
      475 +`recusado` (passo 5), e não duplicatas (passo 6). A nota (pass
          +o 5) vale antes do                                            
      476 +limite (passo 8), o que atende à AMB-021.                     
      477 +                                                              
      478  ## 9. Critérios de aceite
      479  
      480  O sistema está pronto quando:
      481  
       96 -- [ ] <critério verificável, sem ler código>                  
       97 -- [ ] <...>                                                   
      482 +- [ ] Para o arquivo `exemplos/despesas-exemplo.json`, `total_
          +reembolsavel` é 581,53 e `viagem` é verdadeiro.               
      483 +- [ ] No mesmo arquivo, os itens aprovados são d-001 (72,50), 
          +d-002 (17,50), d-003 (100,00), d-010 (250,00), d-011 (33,33), 
          +d-012 (47,20) e d-014 (61,00).                                
      484 +- [ ] No mesmo arquivo, d-004, d-005, d-009 e d-013 são `recus
          +ado`, e d-006, d-007 e d-008 são `em_revisao`, todos com 0,00.
      485 +- [ ] `resumo` indica 7 `aprovado`, 4 `recusado` e 3 `em_revis
          +ao`, e a lista de itens tem 14 elementos, na ordem da entrada.
      486 +- [ ] d-001 e d-002 sozinhas pagam 60,00 e 0,00; d-003 sozinha
          + paga 80,00; d-014 sozinha paga 60,00.                        
      487 +- [ ] Sem d-010 e d-013, `viagem` é falso.                    
      488 +- [ ] Todo item tem justificativa e lista de regras que citam 
          +RN existentes nesta spec.                                     
      489 +- [ ] Todo item `em_revisao` ou `recusado` tem `valor_reembols
          +avel` 0,00.                                                   
      490 +- [ ] Executar duas vezes com a mesma entrada produz a mesma s
          +aída.                                                         
      491  
      492  ## 10. O que fica em aberto
      493  
      101 -<Perguntas que você não conseguiu responder e a decisão provis
          -ória que tomou.                                               
      102 -Honestidade aqui vale ponto — spec que finge não ter buraco é 
          -spec que esconde buraco.>                                     
      494 +- **AMB-011:** quando `competencia` e `inicio`/`fim` divergem,
          + não há regra definida.                                       
      495 +- **AMB-020:** o número não é usado na numeração das ambiguida
          +des. Não há decisão registrada.                               
      496 +- **Categoria irreconhecível × categoria fora da política (AMB
          +-013 × AMB-018):** não há critério que diferencie os dois caso
          +s. Decisão provisória: ambos ficam `recusado`, com alerta de e
          +ncaminhamento para análise humana.                            
      497 +- **Saldo diário esgotado:** o item que recebe 0,00 por saldo 
          +esgotado fica `recusado` (provisório).                        
      498 +- **Fornecedor na duplicata:** a comparação é por igualdade li
          +teral do texto (provisório).                                  
      499 +- **Campos obrigatórios ausentes, arquivo inválido ou valor ig
          +ual a zero:** sem regra definida.                             

● Escrevi specs/001-motor-reembolso/spec.md do zero, a partir do template. Só
  usei as suas decisões, sem commit. Não rodei nada nem conferi o arquivo por
  script. Os números de aceite foram calculados à mão.

  Resultado do arquivo completo, pelas suas regras:
  - O total reembolsável é 581,53, com viagem verdadeiro, porque d-010 é
    aprovada.
  - São 7 aprovado: d-001 (72,50), d-002 (17,50), d-003 (100,00), d-010
    (250,00), d-011 (33,33), d-012 (47,20) e d-014 (61,00).
  - São 4 recusado: d-004 e d-013 por falta de nota, d-005 por categoria e d-009
    por estorno.
  - São 3 em_revisao: d-006 e d-007 como duplicata, e d-008 por estar fora do
    período.

  Duas consequências que talvez você não esperasse:
  - Como d-010 ativa a viagem, todo o mês de julho usa os limites ampliados (90
    e 120), e o 60 e o 80 aparecem só nos casos "sem viagem". d-001 e d-002
    pagam 72,50 + 17,50, e não 60 + 0.
  - A AMB-004 dizia que d-010 "cabe" (480 em 2 diárias), mas a AMB-015, que você
    escolheu, faz d-010 pagar 250 e cortar 230. Registrei isso no texto da
    AMB-004.

  Quatro pontos em que decidi sem ter a sua resposta. Todos estão na seção 10
  como provisórios:
  - AMB-013 manda a categoria irreconhecível para análise e a AMB-018 manda
    recusar. Não há critério para separar os dois casos, então ambos ficam
    recusado, com alerta de encaminhamento.
  - Item que fica com saldo diário esgotado vira recusado com 0,00.
  - Duplicata compara o fornecedor por texto literal.
  - AMB-020 fica registrada como lacuna.

  O que não fiz: não toquei no DECISIONS.md. Se quiser, registro lá a escolha da
  AMB-015 sobre a AMB-004 e proponho o docs(spec) do commit.

✻ Cooked for 2m 31s · done 16:15

※ recap: Estamos montando o motor de reembolso em modo spec-driven, e acabei de escrever o `spec.md` com as 23 decisões, sem commit. O próximo passo é você revisar a spec e, se aprovar, eu registro no `DECISIONS.md` e faço o commit `docs(spec)`. (disable recaps in /config)