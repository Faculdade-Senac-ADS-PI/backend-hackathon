# PRJ22 — Método e referência

Documento sintético. Data de corte: 2025-03-17.

## 1. Referência anterior

Persistir última tela e usar chave idempotente evita duplicar a mesma requisição, mas não resolve quando a confirmação financeira chega antes do registro da retomada e a tela antiga ainda pode emitir outra intenção. Uma saga de compensação foi comparada e gerou confirmação inconsistente nesse intervalo.

Os catálogos/manuais citados neste documento são extratos fictícios definidos acima; não são documentos internos de instituição real. O trecho fornecido descreve as funções relevantes à comparação.

## 2. Mecanismo e hipótese

Marcador contém intenção imutável, época da jornada e prova de confirmação. Antes de retomar, cruzar estado local com prova financeira; se houver confirmação sem marcador final, reconciliar sem reemitir. Se a prova ficar indisponível, manter bloqueio de intenção e exigir confirmação assistida. O protótipo introduz barreira por época entre recuperação de tela e autorização de nova intenção, além da idempotência do pagamento.

## 3. Protocolo e critérios

120 jornadas: 40 interrupções antes, 40 durante e 40 depois da confirmação. Comparadores: última tela+idempotência, saga e barreira por época. Na barreira, 116 retomam automaticamente; quatro aguardam confirmação assistida; nenhuma transferência é duplicada. Há rastros da corrida que a idempotência isolada não resolveu.

## 4. Parâmetros, versões e execução registrada

- semente: 202522
- pontos_interrupcao: ["antes", "durante", "depois"]
- timeout_prova_s: 15
- epoca_persistida: true

Versões localizadas: tela-v1, saga-v2, barreira-v3. Configuração estruturada: `configuracao.json`. Datas e estados: `cronologia.csv`.

## 5. Leitura e reconstrução dos resultados

`medicoes.csv` é a fonte dos contadores por cenário e das medições numéricas entregues. `resultados.csv` é calculado a partir dela, não é uma segunda confirmação independente. Para contadores, somar numeradores e denominadores apenas dentro de um ensaio. Ensaios diferentes podem compartilhar a população. Em resultados.csv, base_de_calculo preserva o total de referência e descricao_base explica o que ele representa. Para média, mediana e diferença entre maior e menor valor, usar a operação indicada. Para percentil_95 de histograma, ordenar valores e localizar o posto teto(0,95 × soma dos pesos). valor_observado reproduz uma medição única. indicador_precalculado transcreve um indicador já calculado: conferir essa transcrição não reproduz seu cálculo original. O LEIA_ME contém o dicionário completo.

Os arquivos não contêm transações bancárias nem uma implementação completa do sistema descrito. São registros primários fictícios do exercício. `entradas.csv` e `observacoes.csv` contêm recortes explicitamente identificados; não pretendem conter todas as entradas da população de um contador. A semente, quando informada, identifica o desenho sintético do cenário, não um executável ausente.

## 6. Limite da conclusão

A hipótese original também inclui perda do armazenamento local enquanto a prova financeira está temporariamente inacessível. Os quatro estados foram bloqueados, mas não há ensaio da reconstrução da época depois dessa perda. A recuperação nesse subcenário continua sem validação.

## 7. Continuidade e detalhamento técnico

Executar a simulação de perda do armazenamento local durante indisponibilidade da prova financeira e acompanhar a reconstrução da época de sessão.

As especificações descrevem o recorte sintético e não equivalem a um programa executável de produção.
