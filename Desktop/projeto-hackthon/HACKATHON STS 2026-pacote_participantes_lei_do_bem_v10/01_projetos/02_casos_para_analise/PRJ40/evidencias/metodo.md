# PRJ40 — Método e referência

Documento sintético. Data de corte: 2025-05-19.

## 1. Referência anterior

Substituir ausente por zero, imputar mediana e bloquear toda proposta eram referências conhecidas. Nenhuma explicita quanto da confiança provém da combinação de fontes ausentes correlacionadas.

Os catálogos/manuais citados neste documento são extratos fictícios definidos acima; não são documentos internos de instituição real. O trecho fornecido descreve as funções relevantes à comparação.

## 2. Mecanismo e hipótese

Propagar intervalo de pontuação usando limites condicionais por padrão de ausência e dependência entre fontes. Se o intervalo atravessar o limiar decisório 0,60, sinalizar revisão; se ficar de um lado, manter decisão com largura registrada. Comparar zero, mediana e intervalo. Sinalização representa intervalo que cruza o limiar; a mudança efetiva de decisão é apurada contra a pontuação completa reservada.

## 3. Protocolo e critérios

30 mil propostas em 16 padrões de ausência definidos na matriz. A referência completa fica reservada para avaliação. O intervalo sinaliza 412 casos que zero/mediana tratavam como confiantes; 228 desses mudam de lado quando a referência completa é revelada. Há 184 sinalizações conservadoras sem mudança de decisão. Métodos usam o mesmo conjunto.

## 4. Parâmetros, versões e execução registrada

- semente: 202540
- padroes: 16
- limiar: 0.6
- tripla_rara_executada: false

Versões localizadas: intervalo-v1. Configuração estruturada: `configuracao.json`. Datas e estados: `cronologia.csv`.

## 5. Leitura e reconstrução dos resultados

`medicoes.csv` é a fonte dos contadores por cenário e das medições numéricas entregues. `resultados.csv` é calculado a partir dela, não é uma segunda confirmação independente. Para contadores, somar numeradores e denominadores apenas dentro de um ensaio. Ensaios diferentes podem compartilhar a população. Em resultados.csv, base_de_calculo preserva o total de referência e descricao_base explica o que ele representa. Para média, mediana e diferença entre maior e menor valor, usar a operação indicada. Para percentil_95 de histograma, ordenar valores e localizar o posto teto(0,95 × soma dos pesos). valor_observado reproduz uma medição única. indicador_precalculado transcreve um indicador já calculado: conferir essa transcrição não reproduz seu cálculo original. O LEIA_ME contém o dicionário completo.

Os arquivos não contêm transações bancárias nem uma implementação completa do sistema descrito. São registros primários fictícios do exercício. `entradas.csv` e `observacoes.csv` contêm recortes explicitamente identificados; não pretendem conter todas as entradas da população de um contador. A semente, quando informada, identifica o desenho sintético do cenário, não um executável ausente.

## 6. Limite da conclusão

A matriz não contém ausência conjunta das três fontes raras R1,R2,R3, embora o objetivo original incluísse esse ramo. Os limites condicionais nesse ramo não foram estimados, portanto não se sustenta cobertura geral de ausência. Comparação futura com outros estimadores não substitui essa limitação específica.

## 7. Continuidade e detalhamento técnico

Executar a estimação dos limites condicionais para a ausência conjunta de R1, R2 e R3 e confrontar as sinalizações com a referência completa.

As especificações descrevem o recorte sintético e não equivalem a um programa executável de produção.
