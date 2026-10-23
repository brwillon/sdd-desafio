 ▐▛███▛█   Claude Code v2.1.309
▝▜██████▀  Sonnet 5.5 · Claude Pro
 ▝▝   ▝▝   D:\github\sdd-desafio


❯ /clear                                                                        

❯ Leia a task T-007 em specs/001-motor-reembolso/tasks.md e as RN/AMB que ela   
  cita na spec.                                                                 
  Escreva primeiro o teste xUnit (nome RNxxx_Cenario_Resultado), rode dotnet    
  test e mostre que falha. NÃO implemente ainda.                                

  Searched for 4 patterns, read 8 files (ctrl+o to expand)

● Write(tests\Reembolso.Tests\DuplicatasTests.cs)
  ⎿  Wrote 170 lines to tests\Reembolso.Tests\DuplicatasTests.cs
       1 using Reembolso.Cli;
       2 using Reembolso.Core;
       3 
       4 namespace Reembolso.Tests;
       5 
       7 {
       8     [Fact]
       9     public void RN005_D006D007_EmRevisaoComAlertaCitandoOsDoisId
         s()
      10     {
     … +179 lines (ctrl+o to expand)

  Ran 1 shell command (ctrl+o to expand)

● Escrevi os testes da T-007 e o dotnet test falha, como esperado. Não
  implementei nada.

  O que li: a T-007 (passo 6 da seção 8) cita RN-005, RN-010, AMB-007 e AMB-008.
  Li também a seção 8 da spec e o exemplos/despesas-exemplo.json.

  O que escrevi: tests/Reembolso.Tests/DuplicatasTests.cs, com os 7 testes de
  aceite da task, nos nomes exatos:
  - RN005_D006D007_EmRevisaoComAlertaCitandoOsDoisIds
  - RN005_D001D002FornecedoresDiferentes_NaoFormamGrupo
  - RN005_D003D004ValoresDiferentes_NaoFormamGrupo
  - RN005_GrupoDeTres_TodosEmRevisaoComAlertaCitandoOsTres
  - RN010_EmRevisao_NaoConsomeSaldoDiario
  - AMB007_CriterioEhDataCategoriaFornecedorEValor
  - SEC8_IguaisSemNotaAcima100_AmbasRecusadasPorRN004

  Resultado do dotnet test: falha na compilação (CS0246), porque o tipo
  Duplicatas ainda não existe. Ele aparece nas linhas 127, 152 e 160 do arquivo
  de teste. Não rodou nenhum teste individual, então "falhou" aqui significa que
  o código não compila, não que cada teste ficou vermelho.

  Decisões minhas sobre o que o teste espera, para você revisar:
  - Nome do tipo: Duplicatas, em Reembolso.Core, com Aplicar(ContextoAvaliacao),
    no mesmo padrão de NotaFiscalObrigatoria.
  - Alerta: vai em ItemAvaliado.Alertas (hoje ninguém o preenche). Cada item do
    grupo recebe um único alerta com todos os ids do grupo, e a regra "RN-005"
    entra em Regras. Os testes só checam que o texto contém os ids, não o texto
    exato.
  - Categoria: a comparação usa ItemAvaliado.Categoria, a normalizada, como diz
    a RN-005.
  - Limitação do teste de RN-010: o saldo diário só existe no passo 8 (T-009).
    Por isso ele só confere que o item em revisão sai do passo 6 com
    ValorReembolsavel 0,00 e EmRevisao. O consumo de saldo em si só dá para
    testar na T-009.

  Se esses pontos estiverem certos, o próximo passo é implementar Duplicatas.

✻ Cogitated for 1m 2s · done 11:17

❯ Revise o teste: ele testa a regra da spec ou o que o código faria?            

● Em parte testa a regra, e em parte testa o que eu já imaginava para o código.
  Encontrei quatro problemas.

  Testa a regra da spec
  - RN005_D006D007_... e RN005_GrupoDeTres_...: a spec manda os itens do grupo
    irem para em_revisao, com alerta que cita os ids.
  - RN005_D003D004ValoresDiferentes_...: d-003 e d-004 só diferem no valor,
    então o teste isola esse campo.
  - AMB007_...: troca um campo do critério por vez e confere que o grupo se
    desfaz. Também confere que descrição e nota fiscal não impedem o grupo.
  - SEC8_...: é a consequência escrita na seção 8, e o teste não depende de
    nenhum detalhe interno.

  Testa o que o código faria
  1. Assert.Single(item.Alertas): a spec só diz "alerta que cita os ids do
     grupo". Exigir exatamente um alerta por item é palpite meu sobre a
     implementação. Um código com dois alertas, conforme a spec, falharia. Devo
     trocar por Assert.NotEmpty mais "algum alerta contém todos os ids".
  2. RN010_EmRevisao_NaoConsomeSaldoDiario não testa RN-010:
     - ValorReembolsavel == 0m já é o valor inicial de ItemAvaliado.De. Essa
       asserção passaria mesmo se Duplicatas fizesse nada com o valor.
     - O resto repete o teste de RN-005.
     - O nome promete consumo de saldo, que só existe no passo 8. Hoje ele é uma
       cobertura falsa da RN-010.
     - Prefiro renomear ou mover para a T-009, com um teste de verdade: um item
       em revisão antes de um aprovado não reduz o saldo do dia.
  3. RN005_D001D002FornecedoresDiferentes_... não isola o fornecedor: d-001 vale
     72,50 e d-002 vale 38,00, então os valores também diferem. O teste
     passaria mesmo se o código ignorasse o fornecedor. O AMB007 cobre isso, mas
     este teste, pelo nome, deveria cobrir por conta própria.
  4. Falta o que a spec diz sobre "itens que chegam a este passo": não há teste
     de que um item já recusado ou em_revisao fica fora do grupo. O SEC8 cobre
     só o caso dos dois recusados. Falta, por exemplo, um item ativo igual a um
     em_revisao por RN-003, que deve continuar ativo.

  Posso aplicar essas quatro correções e rodar de novo. Seria o item 2
  renomeado, sem a asserção vazia, e o item 4 como teste novo. Isso exige
  ajustar a lista de aceite da T-007 em tasks.md (docs(tasks):), já que muda os
  nomes dos testes. Quer que eu faça?

✻ Baked for 19s · done 11:20

❯ Sim                                                                           

● Update(tests\Reembolso.Tests\DuplicatasTests.cs)
  ⎿  Added 14 lines, removed 3 lines
      16  
      17              Assert.Equal(Situacao.EmRevisao, item.Situacao);
      18              Assert.Contains("RN-005", item.Regras);
      19 -            var alerta = Assert.Single(item.Alertas);          
      20 -            Assert.Contains("d-006", alerta);                  
      21 -            Assert.Contains("d-007", alerta);                  
      19 +            AssertAlertaCita(item, "d-006", "d-007");          
      20          }
      21      }
      22  
     ...
      36      }
      37  
      38      [Fact]
      39 +    public void RN005_SoOFornecedorDiferente_NaoFormaGrupo()   
      40 +    {                                                          
      41 +        // d-001 e d-002 também diferem no valor; aqui apenas o
         + fornecedor muda.                                              
      42 +        var resultado = Aplicar(Item("d-a", fornecedor: "Bistro
         + Central"), Item("d-b", fornecedor: "Cantina do Porto"));      
      43 +                                                               
      44 +        Assert.All(resultado, i =>                             
      45 +        {                                                      
      46 +            Assert.Equal(Situacao.Ativo, i.Situacao);          
      47 +            Assert.DoesNotContain("RN-005", i.Regras);         
      48 +        });                                                    
      49 +    }                                                          
      50 +                                                               
      51 +    [Fact]                                                     
      52      public void RN005_D003D004ValoresDiferentes_NaoFormamGrupo(
          )
      53      {
      54          // d-003 (100,00) e d-004 (100,01) têm mesma data, cate
          goria e fornecedor, mas valores distintos.

● Update(tests\Reembolso.Tests\DuplicatasTests.cs)
  ⎿  Added 26 lines, removed 8 lines
      75          {
      76              Assert.Equal(Situacao.EmRevisao, i.Situacao);
      77              Assert.Contains("RN-005", i.Regras);
      78 -            var alerta = Assert.Single(i.Alertas);             
      79 -            Assert.Contains("d-a", alerta);                    
      80 -            Assert.Contains("d-b", alerta);                    
      81 -            Assert.Contains("d-c", alerta);                    
      78 +            AssertAlertaCita(i, "d-a", "d-b", "d-c");          
      79          });
      80      }
      81  
      82      [Fact]
      86 -    public void RN010_EmRevisao_NaoConsomeSaldoDiario()        
      83 +    public void RN010_DuplicataEmRevisao_ZeraValorReembolsavel(
         +)                                                              
      84      {
      88 -        // O saldo diário só existe no passo 8 (T-009). Aqui se
         - verifica o que o torna possível:                              
      89 -        // o item em revisão sai do passo 6 com valor reembolsá
         -vel 0,00 e fora da situação ativa.                             
      90 -        var resultado = Aplicar(Item("d-a", valor: 54.90m), Ite
         -m("d-b", valor: 54.90m));                                      
      85 +        // O consumo de saldo diário é do passo 8 (T-009). Aqui
         +: valor pré-existente é zerado.                                
      86 +        var a = Item("d-a") with { ValorReembolsavel = 54.90m }
         +;                                                              
      87 +        var b = Item("d-b") with { ValorReembolsavel = 54.90m }
         +;                                                              
      88  
      89 +        var resultado = Aplicar(a, b);                         
      90 +                                                               
      91          Assert.All(resultado, i =>
      92          {
      93              Assert.Equal(Situacao.EmRevisao, i.Situacao);
     ...
       95          });
       96      }
       97  
       98 +    [Theory]                                                  
       99 +    [InlineData(Situacao.Recusado)]                           
      100 +    [InlineData(Situacao.EmRevisao)]                          
      101 +    public void RN005_ItemNaoAtivo_NaoParticipaDoGrupo(Situaca
          +o situacao)                                                   
      102 +    {                                                         
      103 +        var inativo = Item("d-a") with { Situacao = situacao, 
          +Regras = ["RN-003"] };                                        
      104 +        var ativo = Item("d-b");                              
      105 +                                                              
      106 +        var resultado = Aplicar(inativo, ativo);              
      107 +                                                              
      108 +        var depoisInativo = resultado.Single(i => i.Despesa.Id
          + == "d-a");                                                   
      109 +        var depoisAtivo = resultado.Single(i => i.Despesa.Id =
          += "d-b");                                                     
      110 +        Assert.Equal(situacao, depoisInativo.Situacao);       
      111 +        Assert.DoesNotContain("RN-005", depoisInativo.Regras);
      112 +        Assert.Equal(Situacao.Ativo, depoisAtivo.Situacao);   
      113 +        Assert.DoesNotContain("RN-005", depoisAtivo.Regras);  
      114 +        Assert.Empty(depoisAtivo.Alertas);                    
      115 +    }                                                         
      116 +                                                              
      117      [Fact]
      118      public void AMB007_CriterioEhDataCategoriaFornecedorEValor
           ()
      119      {

● Update(tests\Reembolso.Tests\DuplicatasTests.cs)
  ⎿  Added 6 lines
      164          });
      165      }
      166  
      167 +    private static void AssertAlertaCita(ItemAvaliado item, pa
          +rams string[] ids)                                            
      168 +    {                                                         
      169 +        Assert.Contains("RN-005", item.Regras);               
      170 +        Assert.Contains(item.Alertas, alerta => ids.All(alerta
          +.Contains));                                                  
      171 +    }                                                         
      172 +                                                              
      173      private static ItemAvaliado Item(
      174          string id,
      175          DateOnly? data = null,

  Searched for 1 pattern, read 1 file (ctrl+o to expand)

● Update(specs\001-motor-reembolso\tasks.md)
  ⎿  Added 1 line, removed 1 line
      52  
      53  - [ ] **T-007** — Duplicatas e itens em revisão (passo 6)
      54    - **Atende:** RN-005, RN-010, AMB-007, AMB-008
      55 -  - **Aceite:** `RN005_D006D007_EmRevisaoComAlertaCitandoOsDois
         -Ids`, `RN005_D001D002FornecedoresDiferentes_NaoFormamGrupo`, `R
         -N005_D003D004ValoresDiferentes_NaoFormamGrupo`, `RN005_GrupoDeT
         -res_TodosEmRevisaoComAlertaCitandoOsTres`, `RN010_EmRevisao_Nao
         -ConsomeSaldoDiario`, `AMB007_CriterioEhDataCategoriaFornecedorE
         -Valor` e `SEC8_IguaisSemNotaAcima100_AmbasRecusadasPorRN004` pa
         -ssam.                                                          
      55 +  - **Aceite:** `RN005_D006D007_EmRevisaoComAlertaCitandoOsDois
         +Ids`, `RN005_D001D002FornecedoresDiferentes_NaoFormamGrupo`, `R
         +N005_D003D004ValoresDiferentes_NaoFormamGrupo`, `RN005_GrupoDeT
         +res_TodosEmRevisaoComAlertaCitandoOsTres`, `RN005_SoOFornecedor
         +Diferente_NaoFormaGrupo`, `RN005_ItemNaoAtivo_NaoParticipaDoGru
         +po`, `RN010_DuplicataEmRevisao_ZeraValorReembolsavel`, `AMB007_
         +CriterioEhDataCategoriaFornecedorEValor` e `SEC8_IguaisSemNotaA
         +cima100_AmbasRecusadasPorRN004` passam.                        
      56    - **Commit:** `<hash preenchido depois>`
      57  
      58  - [ ] **T-008** — Caracterização de viagem (passo 7)

● Update(specs\001-motor-reembolso\tasks.md)
  ⎿  Added 1 line, removed 1 line
      62  
      63  - [ ] **T-009** — Limite diário de alimentação e transporte urb
          ano (passo 8)
      64    - **Atende:** RN-007, AMB-001, AMB-002, AMB-003, AMB-010, AMB
          -019, AMB-022, AMB-023
      65 -  - **Aceite:** `RN007_D001D002EmViagem_Pagam7250E1750`, `RN007
         -_D001D002SemViagem_Pagam6000E0Recusado`, `RN007_D003Sozinha_Pag
         -a80`, `RN007_D014Sozinha_Paga60`, `RN007_ArquivoCompleto_D003Pa
         -ga100_D011Paga3333_D012Paga4720_D014Paga6100`, `RN006_Viagem_Am
         -pliaLimitesDeAlimentacaoETransporte`, `AMB001_SaldoPorDiaECateg
         -oria_NaoPorDespesa`, `AMB001_AlimentacaoETransporteNaoCompartil
         -hamSaldo`, `AMB001_SaldoNaoPassaDeUmDiaParaOutro`, `AMB002_Cort
         -eSegueAOrdemDoArquivo`, `AMB003_PagaOLimiteECortaOExcedente`, `
         -AMB010_AlimentacaoIgualAoLimite60_PagaIntegral`, `AMB019_D012Sa
         -bado_SemTratamentoEspecial`, `AMB022_D001AlmocoComCliente_Aprov
         -adoNormalmente` e `AMB023_D011CafeDaManha_EntraNoLimiteDeAlimen
         -tacao` passam.                                                 
      65 +  - **Aceite:** `RN007_D001D002EmViagem_Pagam7250E1750`, `RN007
         +_D001D002SemViagem_Pagam6000E0Recusado`, `RN007_D003Sozinha_Pag
         +a80`, `RN007_D014Sozinha_Paga60`, `RN007_ArquivoCompleto_D003Pa
         +ga100_D011Paga3333_D012Paga4720_D014Paga6100`, `RN010_EmRevisao
         +_NaoConsomeSaldoDiario`, `RN006_Viagem_AmpliaLimitesDeAlimentac
         +aoETransporte`, `AMB001_SaldoPorDiaECategoria_NaoPorDespesa`, `
         +AMB001_AlimentacaoETransporteNaoCompartilhamSaldo`, `AMB001_Sal
         +doNaoPassaDeUmDiaParaOutro`, `AMB002_CorteSegueAOrdemDoArquivo`
         +, `AMB003_PagaOLimiteECortaOExcedente`, `AMB010_AlimentacaoIgua
         +lAoLimite60_PagaIntegral`, `AMB019_D012Sabado_SemTratamentoEspe
         +cial`, `AMB022_D001AlmocoComCliente_AprovadoNormalmente` e `AMB
         +023_D011CafeDaManha_EntraNoLimiteDeAlimentacao` passam.        
      66    - **Commit:** `<hash preenchido depois>`
      67  
      68  - [ ] **T-010** — Limite de hospedagem por lançamento (passo 8)

● Update(specs\001-motor-reembolso\tasks.md)
  ⎿  Added 1 line, removed 1 line
      121  | RN-007 | T-009 | `RN007_D001D002EmViagem_Pagam7250E1750`, `R
           N007_D001D002SemViagem_Pagam6000E0Recusado`, `RN007_D003Sozinh
           a_Paga80`, `RN007_D014Sozinha_Paga60`, `RN007_ArquivoCompleto_
           D003Paga100_D011Paga3333_D012Paga4720_D014Paga6100` |
      122  | RN-008 | T-010 | `RN008_D010_Paga250ECorta230`, `RN008_D013T
           resNoites_RecusadaPorRN004AntesDoLimite` |
      123  | RN-009 | T-002 | `RN009_33333_Vira3333`, `RN009_Empate10125_
           Vira1012`, `RN009_Empate10135_Vira1014` |
      124 -| RN-010 | T-007, T-011 | `RN010_EmRevisao_NaoConsomeSaldoDiar
          -io`, `RN010_RecusadoEEmRevisao_ValorReembolsavelEhZero`, `RN01
          -0_EmRevisao_NaoEntraNoTotal` |                                
      124 +| RN-010 | T-007, T-009, T-011 | `RN010_DuplicataEmRevisao_Zer
          +aValorReembolsavel`, `RN010_EmRevisao_NaoConsomeSaldoDiario`, 
          +`RN010_RecusadoEEmRevisao_ValorReembolsavelEhZero`, `RN010_EmR
          +evisao_NaoEntraNoTotal` |                                     
      125  | RN-011 | T-011 | `RN011_ArquivoCompleto_TotalReembolsavelEh5
           8153`, `RN011_ArquivoCompleto_Resumo7Aprovado4Recusado3EmRevis
           ao`, `RN011_ArquivoCompleto_14ItensNaOrdemDaEntrada`, `RN011_T
           odoItemTemJustificativaEReferenciaRNExistente` |
      126  | AMB-001 | T-009 | `AMB001_SaldoPorDiaECategoria_NaoPorDespes
           a`, `AMB001_AlimentacaoETransporteNaoCompartilhamSaldo`, `AMB0
           01_SaldoNaoPassaDeUmDiaParaOutro` |
      127  | AMB-002 | T-009 | `AMB002_CorteSegueAOrdemDoArquivo` |

● Update(specs\001-motor-reembolso\tasks.md)
  ⎿  Added 1 line, removed 1 line
      116  | RN-002 | T-003 | `RN002_D009Negativo_RecusadoComZero`, `RN00
           2_D009_NaoAlteraSaldoNemTotalDosOutros` |
      117  | RN-003 | T-005 | `RN003_D008ForaDaCompetencia_EmRevisaoComZe
           ro`, `RN003_D014UltimoDiaDoMes_Dentro`, `RN003_PrimeiroDiaDoMe
           s_Dentro` |
      118  | RN-004 | T-006 | `RN004_D003Exato100SemNota_NaoExigeNota`, `
           RN004_D004_10001SemNota_Recusado`, `RN004_D013_690SemNota_Recu
           sado` |
      119 -| RN-005 | T-007 | `RN005_D006D007_EmRevisaoComAlertaCitandoOs
          -DoisIds`, `RN005_D001D002FornecedoresDiferentes_NaoFormamGrupo
          -`, `RN005_D003D004ValoresDiferentes_NaoFormamGrupo`, `RN005_Gr
          -upoDeTres_TodosEmRevisaoComAlertaCitandoOsTres` |             
      119 +| RN-005 | T-007 | `RN005_D006D007_EmRevisaoComAlertaCitandoOs
          +DoisIds`, `RN005_D001D002FornecedoresDiferentes_NaoFormamGrupo
          +`, `RN005_D003D004ValoresDiferentes_NaoFormamGrupo`, `RN005_Gr
          +upoDeTres_TodosEmRevisaoComAlertaCitandoOsTres`, `RN005_SoOFor
          +necedorDiferente_NaoFormaGrupo`, `RN005_ItemNaoAtivo_NaoPartic
          +ipaDoGrupo` |                                                 
      120  | RN-006 | T-008, T-009 | `RN006_ArquivoCompleto_ViagemVerdade
           iro`, `RN006_SemD010ESemD013_ViagemFalso`, `RN006_D013UnicaHos
           pedagem_ViagemFalsoPoisRecusada`, `RN006_Viagem_AmpliaLimitesD
           eAlimentacaoETransporte` |
      121  | RN-007 | T-009 | `RN007_D001D002EmViagem_Pagam7250E1750`, `R
           N007_D001D002SemViagem_Pagam6000E0Recusado`, `RN007_D003Sozinh
           a_Paga80`, `RN007_D014Sozinha_Paga60`, `RN007_ArquivoCompleto_
           D003Paga100_D011Paga3333_D012Paga4720_D014Paga6100` |
      122  | RN-008 | T-010 | `RN008_D010_Paga250ECorta230`, `RN008_D013T
           resNoites_RecusadaPorRN004AntesDoLimite` |

  Ran 1 shell command (ctrl+o to expand)

● Apliquei as quatro correções. O dotnet test continua falhando só porque o tipo
  Duplicatas ainda não existe (CS0246), como antes. Não implementei nada.

  Em DuplicatasTests.cs:
  - Alertas: troquei o Assert.Single por um helper, AssertAlertaCita. Ele exige
    RN-005 em Regras e pelo menos um alerta que contenha todos os ids do grupo,
    sem fixar quantos alertas existem. Os testes de dupla e de trio usam esse
    helper.
  - RN-010: o teste virou RN010_DuplicataEmRevisao_ZeraValorReembolsavel. Os
    itens entram com ValorReembolsavel = 54,90 e a asserção só passa se
    Duplicatas zerar o valor, então ela não é mais vazia.
  - Fornecedor isolado: novo RN005_SoOFornecedorDiferente_NaoFormaGrupo. Os dois
    itens são iguais em tudo menos no fornecedor.
  - Itens não ativos: novo RN005_ItemNaoAtivo_NaoParticipaDoGrupo, com os casos
    Recusado e EmRevisao. Um item não ativo, igual a um ativo, não forma grupo,
    e o ativo continua ativo e sem alerta.

  Em tasks.md, por causa dos nomes novos:
  - T-007: a lista de aceite ganhou os três testes novos e perdeu
    RN010_EmRevisao_NaoConsomeSaldoDiario.
  - T-009: esse teste foi para o aceite da T-009, porque o saldo diário só
    existe no passo 8.
  - Rastreabilidade: atualizei as linhas RN-005 e RN-010 da tabela, e a RN-010
    agora cita a T-009 também.

  Não commitei. A mudança em tasks.md é documentação e, pelo seu CLAUDE.md, pede
  um commit docs(tasks): separado do commit dos testes. Quer que eu faça os
  dois commits, ou prefere seguir direto para a implementação?

✻ Crunched for 41s · done 11:21

❯ realize os 2 commits separados                                                

  Ran 1 shell command (ctrl+o to expand)

● Fiz os dois commits separados e a árvore de trabalho ficou limpa:

  - d694c84 test(T-007): duplicatas e itens em revisão, com
    tests/Reembolso.Tests/DuplicatasTests.cs.
  - e009a2c docs(tasks): ajusta aceite da T-007 e move teste de saldo para a 
    T-009, com specs/001-motor-reembolso/tasks.md.

  Os dois commits terminam com a linha de coautoria. O commit de testes deixa a
  solução sem compilar, já que Duplicatas ainda não existe. É o estado vermelho
  que você pediu.

  Na tasks.md o hash da T-007 continua como <hash preenchido depois>. Quando a
  T-007 for concluída, ele deve apontar para o commit de implementação, como nas
  tasks anteriores.

  O próximo passo é implementar Duplicatas em Reembolso.Core, com um
  feat(T-007).

✻ Sautéed for 9s · done 11:24

❯ Agora implemente o mínimo para o teste passar e rode dotnet test. Se precisar 
  de regra que não está na spec, PARE e me avise. Não faça commit.              

● Write(src\Reembolso.Core\Duplicatas.cs)
  ⎿  Wrote 34 lines to src\Reembolso.Core\Duplicatas.cs
       1 namespace Reembolso.Core;
       2 
       3 public sealed class Duplicatas
       4 {
       5     public ContextoAvaliacao Aplicar(ContextoAvaliacao contexto)
       7         var grupos = contexto.Itens
       8             .Where(i => i.Situacao == Situacao.Ativo)
       9             .GroupBy(i => (i.Despesa.Data, i.Categoria, i.Despes
         a.Fornecedor, i.Despesa.Valor))
      10             .Where(g => g.Count() > 1)
     … +29 lines (ctrl+o to expand)
  Ran 1 shell command (ctrl+o to expand)

● Implementei Duplicatas e o dotnet test passou: 46 aprovados, 0 com falha. Não
  commitei. O arquivo novo é src/Reembolso.Core/Duplicatas.cs.

  Como funciona: entre os itens ainda ativos, agrupa por data, categoria
  normalizada, fornecedor e valor. Todo grupo com 2 ou mais itens vai para
  EmRevisao, com ValorReembolsavel = 0, "RN-005" em Regras e um alerta que lista
  os ids do grupo. Itens recusados ou já em revisão ficam de fora. Rodei só
  dotnet test, não o dotnet format --verify-no-changes.

  Não precisei de regra fora da spec, mas fiz duas escolhas que a spec não
  define:
  - Comparação do fornecedor: é exata, sem normalizar caixa nem espaços. A spec
    diz "o mesmo fornecedor" e a AMB-013 normaliza só a categoria. Se "Bistro
    Central" e "bistro central" devessem ser duplicatas, isso é uma regra nova e
    precisa entrar na spec antes.
  - Texto do alerta: é Possível duplicata: d-006, d-007. A spec só exige que o
    alerta cite os ids. O texto final da saída é assunto da T-011.

  Falta o commit feat(T-007) e a marcação da task como concluída em tasks.md.

✻ Crunched for 15s · done 11:25