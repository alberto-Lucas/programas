# Claude Usage Monitor

Programa para Windows (.exe) que lê os logs do **Claude Code** e mostra o uso em tempo real:

- **Bloco atual de 5 h** (a janela usada pelos limites dos planos Pro/Max): início/fim, tempo até reiniciar,
  tokens, custo, ritmo (tokens/min e US$/h) e projeção até o fim do bloco;
- **Hoje** e **este mês**: tokens de entrada, saída, escrita/leitura de cache, requisições e custo;
- gráfico de **tokens por minuto** da última hora;
- uso **por modelo** no dia e lista das **últimas requisições** (hora, modelo, projeto, tokens, custo).

Atualiza a cada 2 segundos, lendo só as linhas novas dos arquivos.

## De onde vêm os dados

O Claude Code grava cada conversa em arquivos `.jsonl` em `%USERPROFILE%\.claude\projects`
(ou nas pastas indicadas em `CLAUDE_CONFIG_DIR`). O programa só **lê** esses arquivos — não usa
internet nem precisa de chave de API.

Limitações:

- Só aparece o uso feito pelo **Claude Code** neste computador. Conversas no site/app claude.ai não
  ficam gravadas localmente.
- O custo é uma **estimativa com os preços da API**. Em planos Pro/Max é o "equivalente em API", não o
  valor cobrado. O percentual "vs. maior bloco anterior" é só uma referência — a Anthropic não publica
  o limite exato em tokens.

## Como gerar o .exe

Com o [.NET 8 SDK](https://dotnet.microsoft.com/download) instalado, rode `build.bat` ou:

```
dotnet publish -c Release -r win-x64 -o publish
```

O resultado é um único `publish\ClaudeUsageMonitor.exe`, que roda sem precisar instalar o .NET.

Também há um workflow do GitHub Actions (`.github/workflows/claude-usage-monitor.yml`) que gera o
`.exe` e o disponibiliza como artefato da execução.

## Ajustar preços

Copie `precos.exemplo.json` para `precos.json` na mesma pasta do `.exe` e edite os valores
(US$ por milhão de tokens). Campos aceitos: `input`, `output`, `cacheWrite5m`, `cacheWrite1h`,
`cacheRead`. Os de cache, se omitidos, são calculados a partir de `input` (1,25x, 2x e 0,1x).
