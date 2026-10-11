# PRJ29 — Método e referência

Documento sintético. Data de corte: 2025-05-12.

## 1. Referência anterior

Vídeo longo e amostragem fixa de quatro quadros são comparadores. A redução fixa de quadros cabe na memória, mas perde variação temporal suficiente para distinguir replay em câmera simples.

Os catálogos/manuais citados neste documento são extratos fictícios definidos acima; não são documentos internos de instituição real. O trecho fornecido descreve as funções relevantes à comparação.

## 2. Mecanismo e hipótese

Selecionar próximo quadro pela mudança de movimento entre regiões, preservando desafio temporal e limite de seis quadros. A versão seletor-S2-v3 usa variação mínima 0,08; após um replay aceito, seletor-S3-v4 acrescenta vínculo do padrão de movimento ao nonce do desafio. A hipótese combina economia de quadros e resposta temporal, não simples redução de resolução.

## 3. Protocolo e critérios

480 tentativas: 420 legítimas e 60 replays, distribuídas em quatro perfis de aparelho. seletor-S2-v3 conclui 374/420 legítimas (89,05%, arredondado para 89%) e rejeita 59/60 replays. seletor-S3-v4 rejeita o replay que motivou a correção em um reteste; a matriz inteira não foi repetida com seletor-S3-v4.

## 4. Parâmetros, versões e execução registrada

- semente: 202529
- legitimas: 420
- replays: 60
- quadros_max: 6
- limiar_movimento: 0.08
- regressao_S3_completa: false

Versões localizadas: longo-v1, fixo4-v2, seletor-S2-v3, seletor-S3-v4. Configuração estruturada: `configuracao.json`. Datas e estados: `cronologia.csv`.

## 5. Leitura e reconstrução dos resultados

`medicoes.csv` é a fonte dos contadores por cenário e das medições numéricas entregues. `resultados.csv` é calculado a partir dela, não é uma segunda confirmação independente. Para contadores, somar numeradores e denominadores apenas dentro de um ensaio. Ensaios diferentes podem compartilhar a população. Em resultados.csv, base_de_calculo preserva o total de referência e descricao_base explica o que ele representa. Para média, mediana e diferença entre maior e menor valor, usar a operação indicada. Para percentil_95 de histograma, ordenar valores e localizar o posto teto(0,95 × soma dos pesos). valor_observado reproduz uma medição única. indicador_precalculado transcreve um indicador já calculado: conferir essa transcrição não reproduz seu cálculo original. O LEIA_ME contém o dicionário completo.

Os arquivos não contêm transações bancárias nem uma implementação completa do sistema descrito. São registros primários fictícios do exercício. `entradas.csv` e `observacoes.csv` contêm recortes explicitamente identificados; não pretendem conter todas as entradas da população de um contador. A semente, quando informada, identifica o desenho sintético do cenário, não um executável ausente.

## 6. Limite da conclusão

O reteste de um ataque conhecido não constitui validação completa da nova regra. Falta regressão das 480 tentativas em seletor-S3-v4 e conjunto de replay separado do usado no ajuste. A pesquisa em seletor-S2-v3 permanece documentada.

## 7. Continuidade e detalhamento técnico

Executar a submissão de seletor-S3-v4 às tentativas legítimas da regressão e a um conjunto separado de replays, sem reutilizar somente o ataque do ajuste.

As especificações descrevem o recorte sintético e não equivalem a um programa executável de produção.
