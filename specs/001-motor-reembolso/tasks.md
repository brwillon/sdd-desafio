# Tasks — Motor de Cálculo de Reembolso

> Cada task é pequena o bastante para virar **um commit**. Se você não consegue
> descrever o critério de aceite como "o teste X passa", a task está grande demais.
>
> Marque `[x]` conforme conclui — ao longo do caminho, não tudo no fim. O histórico
> de quando cada task foi marcada é lido na correção.

**Formato do commit:** `feat(T-003): <descrição>` · `test(T-003): <descrição>`

Os testes seguem a convenção `RN007_Cenario_Resultado` / `AMB001_...` (CLAUDE.md).
Testes que cobrem a seção 4, 8 ou 9 da spec usam o prefixo `SEC4_`, `SEC8_`, `SEC9_`.
As tasks seguem a ordem da precedência da seção 8 da spec.

---

## Fase 1 — Fundação

- [X] **T-001** — Solução, CLI e leitura do JSON de entrada (seção 4)
  - **Atende:** seção 4 (entrada)
  - **Aceite:** `SEC4_LeExemploOficial_Retorna14DespesasNaOrdemDoArquivo` passa e `dotnet build` conclui sem warnings. O teste lê `exemplos/despesas-exemplo.json` e confere colaborador, competência e 14 despesas.
  - **Commit:** `<hash preenchido depois>`

- [ ] **T-002** — Arredondamento bancário na leitura (passo 1 da seção 8)
  - **Atende:** RN-009, AMB-012
  - **Aceite:** `RN009_33333_Vira3333`, `RN009_Empate10125_Vira1012`, `RN009_Empate10135_Vira1014` e `AMB012_D011_ValorSolicitadoEh3333` passam.
  - **Commit:** `<hash preenchido depois>`

## Fase 2 — Regras de negócio por item

- [ ] **T-003** — Valor negativo (passo 2)
  - **Atende:** RN-002, AMB-017
  - **Aceite:** `RN002_D009Negativo_RecusadoComZero`, `RN002_D009_NaoAlteraSaldoNemTotalDosOutros` e `AMB017_EstornoRecusado_ApenasOItem` passam.
  - **Commit:** `<hash preenchido depois>`

- [ ] **T-004** — Normalização e validação da categoria (passo 3)
  - **Atende:** RN-001, AMB-013, AMB-018
  - **Aceite:** `RN001_D014Maiuscula_NormalizadaParaAlimentacao`, `AMB013_AcentoCaixaEEspacosNasPontas_Normalizados`, `AMB013_EspacoOuHifenEntrePalavras_EquivaleASublinhado`, `AMB018_D005Coworking_RecusadoComZero` e `RN001_CategoriaNaoReconhecida_RecusadaComAlertaDeAnaliseHumana` passam.
  - **Commit:** `<hash preenchido depois>`

- [ ] **T-005** — Período de competência (passo 4)
  - **Atende:** RN-003, AMB-011, AMB-016
  - **Aceite:** `RN003_D008ForaDaCompetencia_EmRevisaoComZero`, `RN003_D014UltimoDiaDoMes_Dentro`, `RN003_PrimeiroDiaDoMes_Dentro`, `AMB011_ComparaPorAnoEMesDaCompetencia` e `AMB016_ComparaPelaDataDaDespesa` passam.
  - **Commit:** `<hash preenchido depois>`

- [ ] **T-006** — Nota fiscal obrigatória acima de 100,00 (passo 5)
  - **Atende:** RN-004, AMB-005, AMB-009, AMB-021
  - **Aceite:** `RN004_D003Exato100SemNota_NaoExigeNota`, `RN004_D004_10001SemNota_Recusado`, `RN004_D013_690SemNota_Recusado`, `AMB005_NotaVerificadaPeloValorIndividualAntesDoCorte`, `AMB009_AcimaDe100EhEstrito` e `AMB021_SemNota_RecusaTotalSemPagamentoParcial` passam.
  - **Commit:** `<hash preenchido depois>`

## Fase 3 — Regras que dependem de outros itens

- [ ] **T-007** — Duplicatas e itens em revisão (passo 6)
  - **Atende:** RN-005, RN-010, AMB-007, AMB-008
  - **Aceite:** `RN005_D006D007_EmRevisaoComAlertaCitandoOsDoisIds`, `RN005_D001D002FornecedoresDiferentes_NaoFormamGrupo`, `RN005_D003D004ValoresDiferentes_NaoFormamGrupo`, `RN005_GrupoDeTres_TodosEmRevisaoComAlertaCitandoOsTres`, `RN010_EmRevisao_NaoConsomeSaldoDiario`, `AMB007_CriterioEhDataCategoriaFornecedorEValor` e `SEC8_IguaisSemNotaAcima100_AmbasRecusadasPorRN004` passam.
  - **Commit:** `<hash preenchido depois>`

- [ ] **T-008** — Caracterização de viagem (passo 7)
  - **Atende:** RN-006, AMB-006, AMB-014
  - **Aceite:** `RN006_ArquivoCompleto_ViagemVerdadeiro`, `RN006_SemD010ESemD013_ViagemFalso`, `RN006_D013UnicaHospedagem_ViagemFalsoPoisRecusada`, `AMB014_HospedagemEmRevisao_NaoCaracterizaViagem` e `AMB006_LimiarDeNotaEHospedagemNaoMudamEmViagem` passam.
  - **Commit:** `<hash preenchido depois>`

- [ ] **T-009** — Limite diário de alimentação e transporte urbano (passo 8)
  - **Atende:** RN-007, AMB-001, AMB-002, AMB-003, AMB-010, AMB-019, AMB-022, AMB-023
  - **Aceite:** `RN007_D001D002EmViagem_Pagam7250E1750`, `RN007_D001D002SemViagem_Pagam6000E0Recusado`, `RN007_D003Sozinha_Paga80`, `RN007_D014Sozinha_Paga60`, `RN007_ArquivoCompleto_D003Paga100_D011Paga3333_D012Paga4720_D014Paga6100`, `RN006_Viagem_AmpliaLimitesDeAlimentacaoETransporte`, `AMB001_SaldoPorDiaECategoria_NaoPorDespesa`, `AMB001_AlimentacaoETransporteNaoCompartilhamSaldo`, `AMB001_SaldoNaoPassaDeUmDiaParaOutro`, `AMB002_CorteSegueAOrdemDoArquivo`, `AMB003_PagaOLimiteECortaOExcedente`, `AMB010_AlimentacaoIgualAoLimite60_PagaIntegral`, `AMB019_D012Sabado_SemTratamentoEspecial`, `AMB022_D001AlmocoComCliente_AprovadoNormalmente` e `AMB023_D011CafeDaManha_EntraNoLimiteDeAlimentacao` passam.
  - **Commit:** `<hash preenchido depois>`

- [ ] **T-010** — Limite de hospedagem por lançamento (passo 8)
  - **Atende:** RN-008, AMB-004, AMB-015, AMB-024
  - **Aceite:** `RN008_D010_Paga250ECorta230`, `RN008_D013TresNoites_RecusadaPorRN004AntesDoLimite`, `AMB004_LimiteDe250EhPorLancamento`, `AMB010_Hospedagem250Exato_PagaIntegral`, `AMB015_DescricaoComDuasDiarias_ContaComoUma` e `AMB024_DiariasLancadasEmDatasDistintas_CadaUmaLimitadaA250` passam.
  - **Commit:** `<hash preenchido depois>`

## Fase 4 — Saída e CLI

- [ ] **T-011** — Totalização, justificativa, regras e alertas na saída (passo 9)
  - **Atende:** RN-011, RN-010, seção 4 (saída)
  - **Aceite:** `RN011_ArquivoCompleto_TotalReembolsavelEh58153`, `RN011_ArquivoCompleto_Resumo7Aprovado4Recusado3EmRevisao`, `RN011_ArquivoCompleto_14ItensNaOrdemDaEntrada`, `RN011_TodoItemTemJustificativaEReferenciaRNExistente`, `RN010_RecusadoEEmRevisao_ValorReembolsavelEhZero`, `RN010_EmRevisao_NaoEntraNoTotal` e `SEC4_SubconjuntoD001D002D004D010_BateComSaidaDaSpec` passam.
  - **Commit:** `<hash preenchido depois>`

- [ ] **T-012** — Saída JSON, determinismo e ponta a ponta (seção 9)
  - **Atende:** seção 4 (saída), seção 9
  - **Aceite:** `SEC4_SaidaJson_PropriedadesNaOrdemDaSecao4E2CasasDecimais`, `SEC9_ArquivoCompleto_ItensAprovadosERecusadosConformeCriterios`, `SEC9_DuasExecucoesComAMesmaEntrada_GeramBytesIdenticos` e `SEC9_CliCalcular_ExemploOficial_GeraArquivoDeSaida` passam.
  - **Commit:** `<hash preenchido depois>`

- [ ] **T-013** — README e matriz de cobertura
  - **Atende:** rastreabilidade (todas as RN e AMB)
  - **Aceite:** `TRACE_TodaRNeAMBDaSpecApareceEmTasks` e `TRACE_CoberturaCitaApenasTestesExistentes` passam. O primeiro lê `spec.md` e `tasks.md`. O segundo confere a tabela "Cobertura" contra os testes do assembly.
  - **Commit:** `<hash preenchido depois>`

---

## Fase 5 — Envelope (criar no Dia 2)

<Novas tasks a partir da mudança de requisito. Numeração continua de onde parou —
não reinicie e não renumere as antigas: a numeração é o eixo da rastreabilidade.>

---

## Pendências de spec (não viram task até serem decididas)

- **Valor igual a zero:** a seção 10 da spec diz "sem regra definida". Nenhuma task o cobre. Se for decidido recusar, corrigir a RN-002 primeiro (`docs(spec):`) e registrar em `DECISIONS.md`.
- **Campos obrigatórios ausentes e arquivo inválido:** mesma situação (seção 10).
- **AMB-011 (divergência entre `competencia` e `inicio`/`fim`):** a T-005 só testa que a comparação usa `competencia`. O comportamento na divergência segue indefinido.
- **AMB-020:** número não usado na spec, sem decisão e sem task.

---

## Cobertura

Preencha ao fechar cada fase. É a sua própria checagem de rastreabilidade — e é
exatamente a matriz que a correção vai montar.

| Regra da spec | Task | Teste |
|---|---|---|
| RN-001 | T-004 | `RN001_D014Maiuscula_NormalizadaParaAlimentacao`, `RN001_CategoriaNaoReconhecida_RecusadaComAlertaDeAnaliseHumana` |
| RN-002 | T-003 | `RN002_D009Negativo_RecusadoComZero`, `RN002_D009_NaoAlteraSaldoNemTotalDosOutros` |
| RN-003 | T-005 | `RN003_D008ForaDaCompetencia_EmRevisaoComZero`, `RN003_D014UltimoDiaDoMes_Dentro`, `RN003_PrimeiroDiaDoMes_Dentro` |
| RN-004 | T-006 | `RN004_D003Exato100SemNota_NaoExigeNota`, `RN004_D004_10001SemNota_Recusado`, `RN004_D013_690SemNota_Recusado` |
| RN-005 | T-007 | `RN005_D006D007_EmRevisaoComAlertaCitandoOsDoisIds`, `RN005_D001D002FornecedoresDiferentes_NaoFormamGrupo`, `RN005_D003D004ValoresDiferentes_NaoFormamGrupo`, `RN005_GrupoDeTres_TodosEmRevisaoComAlertaCitandoOsTres` |
| RN-006 | T-008, T-009 | `RN006_ArquivoCompleto_ViagemVerdadeiro`, `RN006_SemD010ESemD013_ViagemFalso`, `RN006_D013UnicaHospedagem_ViagemFalsoPoisRecusada`, `RN006_Viagem_AmpliaLimitesDeAlimentacaoETransporte` |
| RN-007 | T-009 | `RN007_D001D002EmViagem_Pagam7250E1750`, `RN007_D001D002SemViagem_Pagam6000E0Recusado`, `RN007_D003Sozinha_Paga80`, `RN007_D014Sozinha_Paga60`, `RN007_ArquivoCompleto_D003Paga100_D011Paga3333_D012Paga4720_D014Paga6100` |
| RN-008 | T-010 | `RN008_D010_Paga250ECorta230`, `RN008_D013TresNoites_RecusadaPorRN004AntesDoLimite` |
| RN-009 | T-002 | `RN009_33333_Vira3333`, `RN009_Empate10125_Vira1012`, `RN009_Empate10135_Vira1014` |
| RN-010 | T-007, T-011 | `RN010_EmRevisao_NaoConsomeSaldoDiario`, `RN010_RecusadoEEmRevisao_ValorReembolsavelEhZero`, `RN010_EmRevisao_NaoEntraNoTotal` |
| RN-011 | T-011 | `RN011_ArquivoCompleto_TotalReembolsavelEh58153`, `RN011_ArquivoCompleto_Resumo7Aprovado4Recusado3EmRevisao`, `RN011_ArquivoCompleto_14ItensNaOrdemDaEntrada`, `RN011_TodoItemTemJustificativaEReferenciaRNExistente` |
| AMB-001 | T-009 | `AMB001_SaldoPorDiaECategoria_NaoPorDespesa`, `AMB001_AlimentacaoETransporteNaoCompartilhamSaldo`, `AMB001_SaldoNaoPassaDeUmDiaParaOutro` |
| AMB-002 | T-009 | `AMB002_CorteSegueAOrdemDoArquivo` |
| AMB-003 | T-009 | `AMB003_PagaOLimiteECortaOExcedente` |
| AMB-004 | T-010 | `AMB004_LimiteDe250EhPorLancamento` |
| AMB-005 | T-006 | `AMB005_NotaVerificadaPeloValorIndividualAntesDoCorte` |
| AMB-006 | T-008 | `AMB006_LimiarDeNotaEHospedagemNaoMudamEmViagem` |
| AMB-007 | T-007 | `AMB007_CriterioEhDataCategoriaFornecedorEValor` |
| AMB-008 | T-007 | `RN005_D006D007_EmRevisaoComAlertaCitandoOsDoisIds`, `RN005_GrupoDeTres_TodosEmRevisaoComAlertaCitandoOsTres` |
| AMB-009 | T-006 | `AMB009_AcimaDe100EhEstrito` |
| AMB-010 | T-009, T-010 | `AMB010_AlimentacaoIgualAoLimite60_PagaIntegral`, `AMB010_Hospedagem250Exato_PagaIntegral` |
| AMB-011 | T-005 | `AMB011_ComparaPorAnoEMesDaCompetencia` (divergência com `inicio`/`fim` segue em aberto) |
| AMB-012 | T-002 | `AMB012_D011_ValorSolicitadoEh3333` |
| AMB-013 | T-004 | `AMB013_AcentoCaixaEEspacosNasPontas_Normalizados`, `AMB013_EspacoOuHifenEntrePalavras_EquivaleASublinhado` |
| AMB-014 | T-008 | `AMB014_HospedagemEmRevisao_NaoCaracterizaViagem` |
| AMB-015 | T-010 | `AMB015_DescricaoComDuasDiarias_ContaComoUma` |
| AMB-016 | T-005 | `AMB016_ComparaPelaDataDaDespesa` |
| AMB-017 | T-003 | `AMB017_EstornoRecusado_ApenasOItem` |
| AMB-018 | T-004 | `AMB018_D005Coworking_RecusadoComZero` |
| AMB-019 | T-009 | `AMB019_D012Sabado_SemTratamentoEspecial` |
| AMB-020 | — | Sem decisão na spec; nada a testar |
| AMB-021 | T-006 | `AMB021_SemNota_RecusaTotalSemPagamentoParcial` |
| AMB-022 | T-009 | `AMB022_D001AlmocoComCliente_AprovadoNormalmente` |
| AMB-023 | T-009 | `AMB023_D011CafeDaManha_EntraNoLimiteDeAlimentacao` |
| AMB-024 | T-010 | `AMB024_DiariasLancadasEmDatasDistintas_CadaUmaLimitadaA250` |
| Seção 4 (entrada/saída) | T-001, T-011, T-012 | `SEC4_LeExemploOficial_Retorna14DespesasNaOrdemDoArquivo`, `SEC4_SubconjuntoD001D002D004D010_BateComSaidaDaSpec`, `SEC4_SaidaJson_PropriedadesNaOrdemDaSecao4E2CasasDecimais` |
| Seção 8 (precedência) | T-007 | `SEC8_IguaisSemNotaAcima100_AmbasRecusadasPorRN004` |
| Seção 9 (aceite) | T-012, T-013 | `SEC9_ArquivoCompleto_ItensAprovadosERecusadosConformeCriterios`, `SEC9_DuasExecucoesComAMesmaEntrada_GeramBytesIdenticos`, `SEC9_CliCalcular_ExemploOficial_GeraArquivoDeSaida` |
