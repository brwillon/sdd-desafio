# Motor de Cálculo de Reembolso

CLI em C# 13 / .NET 9 que lê um JSON de despesas corporativas de um colaborador e
emite um JSON com o valor reembolsável e a justificativa de cada item. O projeto
foi feito com Spec Driven Development: a spec manda, o código segue.

## Como rodar

Requer o [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0).

```bash
dotnet run --project src/Reembolso.Cli -- calcular --input exemplos/despesas-exemplo.json --output resultado.json
```

Códigos de saída: `0` sucesso, `1` entrada ilegível, `2` uso incorreto dos argumentos.
A saída é determinística: a mesma entrada gera sempre os mesmos bytes.

## Como testar

```bash
dotnet test                              # todos os testes
dotnet format --verify-no-changes        # lint/format (aplicar: dotnet format)
```

## Estrutura

| Caminho | Conteúdo |
|---|---|
| `src/Reembolso.Core` | Regras puras, sem I/O. Um tipo por passo da precedência (spec, seção 8). |
| `src/Reembolso.Cli` | Único projeto com I/O: leitura do JSON, serialização e linha de comando. |
| `tests/Reembolso.Tests` | Testes xUnit, nomeados `RN007_Cenario_Resultado` e `AMB001_...`. |
| `exemplos/despesas-exemplo.json` | Entrada de referência com 14 despesas. |

## Documentos de especificação

Ficam em `specs/001-motor-reembolso/`:

- [`spec.md`](specs/001-motor-reembolso/spec.md): **o que** o sistema faz (regras RN e decisões de ambiguidade AMB).
- [`plan.md`](specs/001-motor-reembolso/plan.md): **como** é construído.
- [`tasks.md`](specs/001-motor-reembolso/tasks.md): **em que ordem**, com critério de aceite por task.
- [`DECISIONS.md`](specs/001-motor-reembolso/DECISIONS.md): log de mudanças de spec.

## Rastreabilidade

A matriz **Cobertura** no fim de [`tasks.md`](specs/001-motor-reembolso/tasks.md#cobertura)
liga cada regra da spec (RN-001 a RN-011, AMB-001 a AMB-024 e as seções 4, 8 e 9) à task
que a implementa e aos testes que a verificam. Dois testes mantêm a matriz honesta:

- `TRACE_TodaRNeAMBDaSpecApareceEmTasks`: todo ID de RN/AMB citado na spec tem linha na matriz.
- `TRACE_CoberturaCitaApenasTestesExistentes`: todo teste citado na matriz existe no assembly.

A AMB-020 não tem decisão na spec e, por isso, não tem teste.

## Fora de escopo

Pagamento, conversão de moeda, mais de um colaborador por arquivo, estornos e
detecção de refeição com cliente (spec, seção 3).
