# Log de Decisões e Mudanças de Spec

> Uma entrada **toda vez** que a spec mudar. Este arquivo é a prova de que a spec
> foi tratada como artefato vivo e não como cerimônia de abertura.
>
> Spec que não muda em dois dias é spec que ninguém consultou. Mudança não é
> demérito — mudança não registrada é.

Ordem cronológica inversa: a mais recente primeiro.

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

