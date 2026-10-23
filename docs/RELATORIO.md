# Relatório — Desafio SDD

**Aluno:** Wellerson Willon Reis · **Repositório:** https://github.com/brwillon/sdd-desafio · **Data:** 23/10/2026

Estado do repositório quando escrevi isto: HEAD `61442bd`, 13 tasks concluídas, `dotnet test` com 87 testes passando e nenhum falhando. O envelope do Dia 2 ainda não está registrado no repositório; a seção "O envelope" diz o que falta.

Sobre as sessões: todas foram feitas no Claude Code (Sonnet 5.5) e estão em `docs/sessions/`, de 01 a 20, exportadas ao fim de cada uma. Não há lacuna de exportação. As sessões 03 a 05 são curtas porque cada uma aplica uma correção só na spec.

---

## Delegação

**A divisão:**

| Atividade | Quem | Por quê |
|---|---|---|
| Identificar ambiguidades | Claude levantou, eu conferi | Pedi que percorresse o JSON item por item e listasse sem decidir nada. Ele chegou a 22 (`02-spec-ambiguidades.md`, linhas 8-426). Eu li a lista contra o JSON antes de decidir. |
| Decidir as ambiguidades | Eu, com quatro exceções | As decisões estão no prompt da linha 432 da sessão 02, escritas por mim. Quatro itens (três estados de status, ordem da seção 8, só hospedagem aprovada vale como viagem, arredondar na leitura) foram propostas do Claude que aceitei com "proposta aceita" e "ok" (linhas 700 e 755). Estão na spec como decisões, mas a autoria é dele. |
| Escrever a spec | Claude escreveu, eu corrigi | Ele montou o `spec.md` de uma vez (sessão 02, linha 761) a partir das minhas decisões. Eu revi depois e as correções viraram D-001 a D-003. |
| Desenhar a arquitetura | Eu decidi, Claude redigiu | Stack, alternativas descartadas, os quatro DTs e a estratégia de testes estão no meu prompt (`06-plan.md`, linha 8). Ele só preencheu o `plan.md`. |
| Implementar | Claude | Sempre depois do teste vermelho, com a instrução "implemente o mínimo… se precisar de regra que não está na spec, PARE e me avise" (por exemplo `17-...T-010.md`, linha 176). |
| Escrever testes | Claude escreveu, eu revisei | Todo teste foi escrito por ele. Minha revisão foi o prompt "Revise o teste: ele testa a regra da spec ou o que o código faria?", repetido em cada task de regra (T-006 a T-010). |
| Absorver o envelope | Pendente | Ver seção "O envelope". |

**Onde deleguei e me arrependi:** nos commits. Mandei o Claude fazer vários deles (sessões 14, 16, 17, 18 e 20), e as mensagens saíram com rótulo errado em alguns, que não reescrevi. 

**Onde não deleguei e deveria ter delegado:** o relatório que você está lendo é o caso mais claro. Eu estava resolvendo cada task com o Claude e deixei para o fim a reconstrução das evidências, que ele fez em minutos lendo `git log` e as sessões. Se eu tivesse pedido um resumo de evidências ao fim de cada sessão, o relatório teria saído mais fácil.

**Usei subagentes / skills / MCP / hooks?** Não configurei nenhum. 

---

## Descrição

*Requisito escolhido: hospedagem "por diária" (AMB-004 / RN-008).*

**Versão 1 (commit `0f481eb`, 22/10 16:18):**
> ```
> **Decisão:** o limite de 250,00 vale por diária, e a diária conta o número de noites.
> Entretanto, prevalece a AMB-015: cada lançamento é uma única diária, e a quantidade
> escrita na descrição é ignorada. Assim, d-010 (480,00) paga 250,00.
> **Justificativa:** a diária de hospedagem conta noites, e o lançamento individual por
> noite (AMB-015, AMB-024) evita inferir a quantidade do texto.
> ```

**Versão final (commit `05a5eca`, 22/10 17:33, D-001):**
> ```
> **Decisão:** O limite de 250,00 vale por lançamento, e cada lançamento de hospedagem
> equivale a uma diária (AMB-015). Quantidades de noites ou diárias na descrição são
> ignoradas. Assim, d-010 (480,00) paga 250,00.
> **Justificativa:** A descrição é texto livre e não confiável; o lançamento individual
> por diária (AMB-015, AMB-024) evita inferir a quantidade.
> ```

**O que estava ambíguo:** "R$ 250 por diária" não diz se o teto se multiplica pelo número de diárias do lançamento ou vale uma vez por lançamento. O JSON expõe isso em d-010 ("2 diarias", 480,00) e d-013 ("3 noites", 690,00): com teto por noite, d-010 cabe inteira; com teto por lançamento, paga 250,00.

**Como percebi:** a versão 1 tinha duas decisões ao mesmo tempo. Dizia "vale por diária e conta as noites" e logo depois "prevalece a AMB-015, que ignora a quantidade". O resultado numérico (250,00) estava certo, mas quem lesse só a primeira frase calcularia 480,00. O gatilho que registrei na D-001 é uma "revisão de lacunas: cálculo manual feito só com a spec". Antes disso, na sessão 02 (linha 667), eu já tinha decidido "vale AMB-015" ao responder a pergunta do Claude sobre o que fazer com d-010, mas o texto da AMB-004 não foi reescrito na hora.

**Commit da mudança:** `05a5eca` (spec) e `4a1c5d3` (entrada D-001 no `DECISIONS.md`). A sessão está em `docs/sessions/03-correcao-spec-AMB-004.md`.

**Outro caso, mais curto:** a RN-007 dizia que o limite diário era saldo "a consumir", mas não dizia se o saldo era compartilhado entre alimentação e transporte, nem se acumulava de um dia para o outro. Reescrevi em `29ed972` (D-003): saldo próprio por (dia, categoria), sem transferência.

---

## Discernimento

### Caso 1 — teste que copiava o comportamento do código, e uma afirmação que o Claude fez sem conferir

**O que ele propôs:** na T-006 (nota fiscal), o Claude escreveu um teste `RN004_ItemJaEmRevisao_NaoEhAlterado`, com a asserção `DoesNotContain("RN-004")`, e disse que "a RN-004 e a seção 8 pedem" esse teste (`13-implementacao-fundacao-T-006.md`, linhas 84-88). Na T-009 ele afirmou que os valores da RN-007 não davam o total de 581,53 da RN-011 e sugeriu conferir a spec antes da T-011 (`16-implementacao-fundacao-T-009.md`, linhas 56-58).

**Por que estava errado:** a spec não dizia o que acontece com um item sem nota que já saiu do fluxo; a seção 8 só trata a duplicata. O teste copiava o comportamento de `PeriodoCompetencia`, uma regra que só existia no código. Quanto ao total, ele não tinha feito a conta: 72,50 + 17,50 + 100,00 + 33,33 + 47,20 + 61,00 + 250,00 dá exatamente 581,53 (linhas 91-94 da mesma sessão). A spec estava certa e o aviso dele estava errado.

**Como eu detectei:** não detectei lendo o diff linha a linha. Detectei com o prompt fixo "Revise o teste: ele testa a regra da spec ou o que o código faria?" (`13-...T-006.md`, linha 98), que fiz em cada task. Foi a resposta a esse prompt que expôs as duas coisas. No caso do total, ele mesmo se corrigiu nessa revisão. Registro que sem esse prompt eu teria aceitado o teste, porque ele passava.

**O que eu fiz:** pelo `CLAUDE.md`, regra que só existe no teste é bug de spec. Mandei registrar na spec e no `DECISIONS.md` antes de seguir. Resultado: RN-004 passou a dizer que só avalia itens ativos (`2e4c0a4`, D-007), e a mesma lacuna na RN-007 virou D-008 (`418b17a`) com um teste novo no aceite da T-009. Na mesma sessão ele também tinha usado `double` num `Theory` de valor monetário, contra o `CLAUDE.md`; foi trocado por `decimal`.

**Onde está a evidência:** `docs/sessions/13-implementacao-fundacao-T-006.md`, linhas 98-310; `docs/sessions/16-implementacao-fundacao-T-009.md`, linhas 64-100 e 136-349; `specs/001-motor-reembolso/DECISIONS.md`, D-007 e D-008.

### Caso 2 — asserções por analogia (T-010) e exemplo da spec inconsistente (T-011)

**Padrão que eu notei:** o Claude erra mais ao **estender por analogia** uma regra que está escrita para outro passo. Aconteceu três vezes:

- **T-010:** ele copiou da RN-007 a ideia de que item recusado antes não recebe a regra do passo e assertou `DoesNotContain("RN-008")` no d-013, e assertou `Situacao.Aprovado` no d-010 sem que a RN-008 falasse de situação. Na própria revisão ele reconheceu que isso "é uma regra que só está na minha cabeça" (`17-...T-010.md`, linhas 75-93). Aqui escolhi o caminho oposto ao da T-006: **removi as duas asserções em vez de ampliar a spec**, e registrei como D-009 (`69e174e`), dizendo na própria entrada que a spec não mudou.
- **T-011:** o teste exigia `regras: ["RN-004"]` para d-004, como no exemplo da seção 4, mas a normalização já põe RN-001 em toda categoria reconhecida (`18-...T-011.md`, linhas 65-70). O defeito era do exemplo da spec, não do teste. Mandei corrigir a spec primeiro (D-010, `3dd47b4`).
- **T-006:** o caso 1.

O sinal que me deixou em alerta foi o próprio Claude escrever "por analogia" ou "copiei do padrão" na revisão. Passei a ler o relatório de revisão procurando essa expressão.

---

## Diligência

**Meu procedimento de verificação:** em cada task de regra (T-006 a T-010) eu repetia a mesma sequência, que está nos prompts exportados:

1. Mandar o Claude ler a task e escrever **só o teste**, rodar `dotnet test` e mostrar a falha.
2. Pedir a revisão "ele testa a regra da spec ou o que o código faria?".
3. Resolver cada ponto da revisão: ou corrigia a spec e o `DECISIONS.md` primeiro, ou removia a asserção.
4. Só então mandar implementar "o mínimo", com ordem de parar se faltasse regra, e rodar `dotnet test`.
5. Commits separados: `docs(spec)`, `test(T-nnn)`, `feat(T-nnn)`, `docs(tasks)` marcando a task concluída e `docs(sessions)`.

Nas tasks T-003 a T-005 a revisão apareceu como resposta minha a uma decisão que o Claude levantou ("Concordo com a decisão, registre no arquivo decisions"; sessões 10, 11 e 12). Em T-001 e T-002 não pedi revisão. Em T-011 a T-013 não usei o prompt fixo; as decisões saíram da análise que o Claude apresentou antes de implementar.

**Li o diff inteiro em que porcentagem das entregas?** `[PREENCHER: porcentagem real]`. O que as sessões comprovam é que a revisão que fiz de forma sistemática foi a revisão dos **testes**, pelo prompt acima. Não há prompt meu pedindo revisão do código de produção; ele aparece nas sessões como saída do Claude e eu confiei no `dotnet test` e no teste vermelho antes dele.

**O que aceitei sem verificar direito, e o que me custou:**

- Mensagens de commit com rótulo errado (ver Delegação). Custo: rastreio por D-nnn ficou ruim em três commits.
- No prompt da sessão 07 eu mandei citar `AMB-025`, `AMB-026` e `RN-012`, que não existem na spec. O Claude avisou e não os citou (`07-plan-e-tasks.md`, linha 40). Eu tinha digitado de memória, sem reler a spec. Isso quase criou rastro para regras inexistentes.
- Na sessão 03 ele se recusou a escrever o `DECISIONS.md` porque eu tinha dito para não tocar nele, e só escreveu quando mandei (`03-...md`, linhas 71-85). Foi o prompt mal montado que gerou o impasse, e a spec ficou por alguns minutos alterada sem registro.
- O `resultado.json` da raiz está sem commit; é saída da CLI, não parte da entrega.
- O `CLAUDE.md` está no git com extensão em maiúsculas (`CLAUDE.MD`); não verifiquei se isso importa fora do Windows.

**Testes: quem escreveu, e como você sabe que eles testam a coisa certa?** O Claude escreveu todos. Teste escrito pelo mesmo agente que escreve o código passa com facilidade, então usei três controles:

1. **A revisão dos testes contra a spec**, que pegou os casos acima.
2. **Teste vermelho antes do código**, mostrado na sessão.
3. **Teste fraco apontado na revisão:** `AMB019` na T-009 só conferia que d-012 pagava 47,20, valor que passaria em qualquer limite. Foi trocado por um sábado de 100,00 que tem de pagar 60,00 (`16-...T-009.md`, linhas 81-84 e 102-121).

Na T-013 os testes de rastreabilidade foram verificados por mutação: o Claude adulterou a tabela de cobertura, viu os dois testes falharem, e restaurou o arquivo (`20-...T-013.md`, linha 92). Não fiz isso nos demais testes. Pontos fracos conhecidos: o teste `AMB005` só prova a soma 60+60, não a ordem nota-antes-do-corte, que só aparece no pipeline completo; e a seção 10 da spec lista comportamentos sem regra (valor zero, campos ausentes, arquivo inválido), sem teste.

---

## O envelope

*A mudança de requisito do Dia 2.*

> Em virtude do volume de projetos em andamento no momento, não tive disponibilidade mior para concluir a atividade.

**Quantos arquivos toquei na mão:** `[PREENCHER]`
**Quanto tempo levou:** `[PREENCHER]`
**Diff de absorção:** `[PREENCHER]` (`git diff 61442bd HEAD --stat`, onde `61442bd` é o último commit antes do envelope)

**Absorveu de graça:** `[PREENCHER depois de ver a mudança]`. O que já dá para dizer, pela estrutura: cada passo da seção 8 é um tipo próprio no Core, e a ordem é uma lista; uma regra nova que caiba num passo deve tocar um tipo, o `spec.md`, o `tasks.md` e o `DECISIONS.md`.

**Resistiu:** `[PREENCHER]`

**Ordem em que fiz:** `[PREENCHER]`. A ordem que o `CLAUDE.md` e o FAQ pedem é spec → `DECISIONS.md` → tasks → código.

**Se eu tivesse escrito a spec original sabendo desta mudança:** `[PREENCHER]`

**O que a spec me poupou, em concreto:** já posso dizer uma coisa do Dia 1. Dez mudanças de spec (D-001 a D-010) e, em todas, o campo "O que isso invalidou" é "Nada"; em nove delas o código da task ainda não existia (a exceção é a D-004). O custo de cada uma foi de 1 a 2 arquivos de spec, mais o log (campo "Custo" do `DECISIONS.md`).

---

## Fechamento

**Para qual tamanho de projeto isto valeu a pena?** Para este, que é pequeno. A política tem nove frases e a spec acabou com 11 regras e 23 decisões de ambiguidade. Valeu porque as regras interagem (a ordem muda o resultado) e porque a spec pegou erro dos dois lados: o teste que inventava regra (D-007, D-008) e o exemplo da spec que contradizia a RN-001 (D-010).

**Para qual não valeria?** Um script de uma regra só, que eu escrevo e testo em uma tarde. Aqui foram 20 sessões e 80 commits para um motor de 11 regras, e boa parte do tempo foi cerimônia: marcar task, exportar sessão, escrever entrada de log.

**O que eu faria diferente:**

- Pedir revisão do código de produção com a mesma pergunta que fiz para os testes.

**A coisa mais desconfortável que aprendi sobre como eu trabalho com IA:** minha verificação virou um prompt fixo. Ele funcionou, mas me fez depender de o Claude se autoavaliar com honestidade, e os erros que pegamos foram todos erros que ele confessou quando perguntado, não erros que eu achei sozinho no diff. Não sei quantos erros ele teria deixado passar se eu não tivesse perguntado do mesmo jeito.
