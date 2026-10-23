# Log de Decisões e Mudanças de Spec

> Uma entrada **toda vez** que a spec mudar. Este arquivo é a prova de que a spec
> foi tratada como artefato vivo e não como cerimônia de abertura.
>
> Spec que não muda em dois dias é spec que ninguém consultou. Mudança não é
> demérito — mudança não registrada é.

Ordem cronológica inversa: a mais recente primeiro.

---

## D-008 — RN-007 só avalia itens ativos · `2026-10-23`

**Gatilho:** Revisão dos testes da T-009: nenhum teste do aceite provava que um item já `recusado` (por exemplo, pela RN-004) não consome o saldo diário; a RN-007 só dizia "as despesas que chegam a este passo" e a regra geral estava apenas na seção 8.

**O que mudou na spec:** RN-007: item já `recusado` ou `em_revisao` em passo anterior não consome saldo e mantém situação, valor e `regras`, sem receber `RN-007`. Teste novo `RN007_ItemRecusadoAntes_NaoConsomeSaldoDiario`, incluído no aceite da T-009.

**Por quê:** Mesmo raciocínio da D-007: a seção 8 já afirma o comportamento; fixá-lo na regra o torna verificável por teste em vez de viver só no código (CLAUDE.md).

**O que isso invalidou:** Nada. O código da T-009 ainda não foi escrito.

**Tasks afetadas:** T-009 (aceite ganha 1 teste).

**Custo:** 2 arquivos de spec (spec.md, tasks.md) e este log, mais o teste.

---

## D-007 — RN-004 só avalia itens ativos · `2026-10-23`

**Gatilho:** Revisão dos testes da T-006: a RN-004 não dizia o que acontece com um item sem nota acima de 100,00 que já saiu do fluxo (`recusado` ou `em_revisao`) em passo anterior. O teste correspondente refletia o padrão do código da T-005, não a spec.

**O que mudou na spec:** RN-004: a regra só avalia itens ainda ativos; item já `recusado` ou `em_revisao` mantém situação, valor e `regras`, sem receber `RN-004`. O item recusado pela regra tem `regras` contendo `RN-004`.

**Por quê:** A seção 8 afirma que item recusado ou em revisão não participa dos passos seguintes; fixar isso na RN-004 mantém a primeira causa de saída como a registrada e torna o comportamento verificável pelo teste, em vez de viver só no código (CLAUDE.md).

**O que isso invalidou:** Nada. O código da T-006 ainda não foi escrito.

**Tasks afetadas:** T-006 (sem mudança de aceite).

**Custo:** 1 arquivo de spec (spec.md) e este log.

---

## D-006 — Registro de regra e valor do item fora da competência · `2026-10-22`

**Gatilho:** Testes da T-005: a RN-003 não definia o valor reembolsável do item em revisão (só o exemplo d-008) nem se `RN-003` consta em `regras`.

**O que mudou na spec:** RN-003: o item fora da competência fica `em_revisao` com valor reembolsável 0,00 e `regras` contém `RN-003`. A RN-003 não define alerta nem justificativa específicos.

**Por quê:** O teste precisa de um critério verificável, e o valor 0,00 e a regra já aparecem no aceite e na seção 4; fixá-los evita que vivam só no código (CLAUDE.md).

**O que isso invalidou:** Nada. O código da T-005 ainda não foi escrito.

**Tasks afetadas:** T-005 (sem mudança de aceite).

**Custo:** 1 arquivo de spec (spec.md) e este log.

---

## D-005 — Alerta e registro de regra na normalização de categoria · `2026-10-22`

**Gatilho:** Testes da T-003/T-004: a RN-001 não definia o conteúdo do alerta de categoria não reconhecida nem se `RN-001` consta em `regras` dos itens reconhecidos.

**O que mudou na spec:** RN-001: o alerta da categoria não reconhecida contém "análise humana"; a categoria informada é mantida na saída; `regras` contém `RN-001` em toda despesa que passa pelo passo, reconhecida ou não.

**Por quê:** O exemplo da seção 4 já lista `RN-001` em itens aprovados (d-001, d-002), e o texto do alerta precisava de um critério verificável pelo teste.

**O que isso invalidou:** Nada. O código da T-004 ainda não foi escrito; `ItemAvaliado` ganhará `Categoria` e `Alertas`.

**Tasks afetadas:** T-004 (sem mudança de aceite).

**Custo:** 1 arquivo de spec (spec.md) e este log.

---

## D-004 — Justificativa do valor negativo · `2026-10-22`

**Gatilho:** Implementação da T-003: a RN-002 não definia o texto da justificativa, que vai para a saída (seção 4).

**O que mudou na spec:** RN-002: a justificativa do item recusado passa a ser "Valor negativo é dado inválido." e `regras` contém `RN-002`.

**Por quê:** O texto tinha sido escolhido no código; regra de saída não pode viver só no código (CLAUDE.md). Fixá-la na spec torna o teste verificável.

**O que isso invalidou:** Nada. O código da T-003 já usa esse texto.

**Tasks afetadas:** T-003 (sem mudança de aceite).

**Custo:** 1 arquivo (spec.md).

---

## D-003 — Saldo independente por dia e categoria · `2026-10-22`

**Gatilho:** Revisão de lacunas: cálculo manual feito só com a spec.

**O que mudou na spec:** RN-007 e AMB-001: saldos independentes por (dia, categoria), sem transferência.

**Por quê:** A redação anterior deixava aberto se o saldo era acumulado ou compartilhado.

**O que isso invalidou:** Nada (ainda não há código nem tasks).

**Tasks afetadas:** Nenhuma.

**Custo:** 1 arquivo (spec.md).

---

## D-002 — Competência compara AAAA-MM · `2026-10-22`

**Gatilho:** Revisão de lacunas: cálculo manual feito só com a spec.

**O que mudou na spec:** RN-003 e AMB-011: critério de "cair no mês" passa a ser igualdade de ano e mês.

**Por quê:** Era uma suposição do implementador; sem isso o teste de d-014 (31/07) não tem base na spec.

**O que isso invalidou:** Nada (ainda não há código nem tasks).

**Tasks afetadas:** Nenhuma.

**Custo:** 1 arquivo (spec.md).

---

## D-001 — AMB-004 sem contradição · `2026-10-22`

**Gatilho:** Revisão de lacunas: cálculo manual feito só com a spec.

**O que mudou na spec:** AMB-004: Decisão e Justificativa reescritas; removida a frase "a diária conta o número de noites", que contradizia a AMB-015.

**Por quê:** A decisão efetiva já era uma diária por lançamento; o texto confundia o leitor.

**O que isso invalidou:** Nada (ainda não há código nem tasks).

**Tasks afetadas:** Nenhuma.

**Custo:** 1 arquivo (spec.md).

