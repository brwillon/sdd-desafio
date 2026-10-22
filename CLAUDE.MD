# CLAUDE.md

> Este arquivo é lido pelo Claude Code no início de toda sessão. É onde moram as
> convenções que você não quer repetir em todo prompt.
> Substitua os `<...>` e apague o que não usar. Mantenha curto — CLAUDE.md longo
> é CLAUDE.md ignorado.

## O projeto

Motor de cálculo de reembolso de despesas corporativas. CLI que lê um JSON de
despesas e emite um JSON com o valor reembolsável e a justificativa de cada item.

## Fonte da verdade

`specs/001-motor-reembolso/spec.md` define **o que** o sistema faz.
`specs/001-motor-reembolso/plan.md` define **como**.
`specs/001-motor-reembolso/tasks.md` define **em que ordem**.

Quando o código e a spec discordarem, a spec está certa e o código é o bug —
a menos que a spec esteja errada, e nesse caso corrigimos a spec primeiro e
registramos em `DECISIONS.md`.

**Antes de implementar qualquer coisa, leia a task correspondente em `tasks.md`.**
Se o que eu pedi não está coberto por nenhuma task, me avise em vez de implementar.

## Regras de trabalho

- Toda regra de negócio vive na spec, não no chat e não em comentário de código.
- Se eu te explicar uma regra que não está na spec, **pare e me diga isso** antes
  de escrever código. Isso é um bug de spec.
- Todo commit referencia uma task: `feat(T-003): <descrição>`.
  Mudanças de documentação: `docs(spec):`, `docs(plan):`, `docs(tasks):`.
- Nenhuma regra de negócio entra sem teste.

## Stack e comandos

- Linguagem: `C# 13 / .NET 9`
- Rodar: `dotnet run --project src/Reembolso.Cli -- calcular --input exemplos/despesas-exemplo.json --output resultado.json`
- Testes: `dotnet test`
- Lint/format: `dotnet format --verify-no-changes` (aplicar: `dotnet format`)

## Convenções de código

- Solução `Reembolso.sln`: `src/Reembolso.Core` (regras puras, sem I/O; um tipo por passo da precedência da seção 8), `src/Reembolso.Cli` (único projeto com I/O), `tests/Reembolso.Tests` (xUnit).
- Testes nomeados `RN007_Cenario_Resultado` e `AMB001_...`, para fechar a rastreabilidade.
- Valores monetários: `decimal`, nunca `double`/`float`. Arredondamento com `Math.Round(v, 2, MidpointRounding.ToEven)`.
- Nullable habilitado e `TreatWarningsAsErrors` ligado.
- JSON com `System.Text.Json`, propriedades na ordem da seção 4 da spec e números com 2 casas.

## Fora de escopo

- Pagamento, conversão de moeda, mais de um colaborador por arquivo, estornos e detecção de refeição com cliente (spec seção 3).
