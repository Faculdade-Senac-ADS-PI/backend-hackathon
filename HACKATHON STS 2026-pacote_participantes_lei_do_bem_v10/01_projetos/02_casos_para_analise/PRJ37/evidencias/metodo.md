# PRJ37 — Método e referência

Documento sintético. Data de corte: 2025-08-11.

## 1. Referência anterior

Reponderação fixa e calibração por grupo são comparadores implementados. Elas reduzem diferença no período de ajuste, mas os grupos pequenos têm erro instável e a retirada de variáveis não resolve dependência temporal.

Os catálogos/manuais citados neste documento são extratos fictícios definidos acima; não são documentos internos de instituição real. O trecho fornecido descreve as funções relevantes à comparação.

## 2. Mecanismo e hipótese

Objetivo combina perda preditiva e penalidade sobre o limite superior de confiança da diferença de FNR entre grupos; grupos escassos recebem incerteza maior, sem criar regra de aprovação específica para cada grupo. Hipóteses H1 reponderação, H2 calibração e H3 penalidade robusta, oito pesos cada, total 24 versões. Seleção por validação temporal, nunca pelo teste final.

## 3. Protocolo e critérios

120 mil registros: 72 mil de treino, 24 mil de validação, 12 mil de teste temporal e 12 mil de estresse, em partições disjuntas. Quatro grupos sintéticos A–D. FNR = falsos negativos / positivos reais; cada grupo tem mil positivos no teste. FNR da referência: [10; 12; 18,4; 14]%; de H3: [11; 12,2; 14,7; 13]%. Amplitude de 8,4 para 3,7 pontos percentuais. AUC de 0,812 para 0,800: perda de 0,012, ou 1,2 ponto na escala percentual; limite de 0,015. No estresse, FNR de H3 = [12; 13,5; 17,1; 15]%, com amplitude de 5,1 pontos.

## 4. Parâmetros, versões e execução registrada

- semente: 202537
- pesos: [0, 0.1, 0.2, 0.4, 0.6, 0.8, 1.0, 1.2]
- versoes: 24
- particoes: [72000, 24000, 12000, 12000]
- positivos_por_grupo_teste: 1000
- limite_amplitude_pp: 4
- limite_perda_auc: 0.015

Versões localizadas: baseline-v1, H3-peso4-v2. Configuração estruturada: `configuracao.json`. Datas e estados: `cronologia.csv`.

## 5. Leitura e reconstrução dos resultados

`medicoes.csv` é a fonte dos contadores por cenário e das medições numéricas entregues. `resultados.csv` é calculado a partir dela, não é uma segunda confirmação independente. Para contadores, somar numeradores e denominadores apenas dentro de um ensaio. Ensaios diferentes podem compartilhar a população. Em resultados.csv, base_de_calculo preserva o total de referência e descricao_base explica o que ele representa. Para média, mediana e diferença entre maior e menor valor, usar a operação indicada. Para percentil_95 de histograma, ordenar valores e localizar o posto teto(0,95 × soma dos pesos). valor_observado reproduz uma medição única. indicador_precalculado transcreve um indicador já calculado: conferir essa transcrição não reproduz seu cálculo original. O LEIA_ME contém o dicionário completo.

Os arquivos não contêm transações bancárias nem uma implementação completa do sistema descrito. São registros primários fictícios do exercício. `entradas.csv` e `observacoes.csv` contêm recortes explicitamente identificados; não pretendem conter todas as entradas da população de um contador. A semente, quando informada, identifica o desenho sintético do cenário, não um executável ausente.

## 6. Limite da conclusão

Critério original exigia amplitude ≤ 4 pontos tanto no teste temporal quanto no estresse. O primeiro atende, o estresse não. Não existe garantia de equidade geral, e FNR não substitui outros critérios de justiça. A equipe ainda precisa examinar a estabilidade sob a safra extrema antes de ampliar a conclusão.

## 7. Continuidade e detalhamento técnico

Executar o exame dos grupos da safra de estresse com a versão selecionada na validação, mantendo FNR e AUC separados por partição.

As especificações descrevem o recorte sintético e não equivalem a um programa executável de produção.

## 8. Leitura dos indicadores registrados

PRJ37-S04 e PRJ37-S05 contêm um valor AUC previamente calculado por versão, transcrito em medicoes.csv. A base 1 significa um indicador registrado, não um único caso avaliado. O pacote permite conferir a transcrição de 0,812 e 0,800, mas não recalcular a AUC original sem os escores e rótulos individuais. A diferença entre taxas dos quatro grupos usa diferenca_maior_menor, com base 4, sem divisão por quatro.
