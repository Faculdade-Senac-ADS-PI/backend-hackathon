# PRJ35 — Método e referência

Documento sintético. Data de corte: 2025-06-02.

## 1. Referência anterior

O protocolo distingue média histórica, regressão com calendário e modelo por etapa. Pretende validar em ordem temporal com erro máximo ≤ 12 minutos. A série original de 18 meses não foi preservada.

Os catálogos/manuais citados neste documento são extratos fictícios definidos acima; não são documentos internos de instituição real. O trecho fornecido descreve as funções relevantes à comparação.

## 2. Mecanismo e hipótese

Rascunho de notebook propõe duração_prevista=soma de previsões por etapa, com volume previsto antes do início da noite. Um trecho recuperado usa volume_final, disponível apenas depois do encerramento; não há saída executada que permita saber qual variante foi usada na demonstração.

## 3. Protocolo e critérios

Quarenta e uma noites têm duração real e volume final recuperados; 12 possuem previsão anotada sem versão, data de emissão ou corte de treino. Dessas anotações pode-se calcular MAE=18 minutos, mas não estabelecer que eram previsões prospectivas. Memorando informa 18 minutos sem distinguir média de máximo.

## 4. Parâmetros, versões e execução registrada

- meses_planejados: 18
- noites_recuperadas: 41
- noites_com_anotacao: 12
- versao_executada: null
- corte_treino: null

Versões localizadas: previsao-v1, serie-v2. Configuração estruturada: `configuracao.json`. Datas e estados: `cronologia.csv`.

## 5. Leitura e reconstrução dos resultados

`medicoes.csv` é a fonte dos contadores por cenário e das medições numéricas entregues. `resultados.csv` é calculado a partir dela, não é uma segunda confirmação independente. Para contadores, somar numeradores e denominadores apenas dentro de um ensaio. Ensaios diferentes podem compartilhar a população. Em resultados.csv, base_de_calculo preserva o total de referência e descricao_base explica o que ele representa. Para média, mediana e diferença entre maior e menor valor, usar a operação indicada. Para percentil_95 de histograma, ordenar valores e localizar o posto teto(0,95 × soma dos pesos). valor_observado reproduz uma medição única. indicador_precalculado transcreve um indicador já calculado: conferir essa transcrição não reproduz seu cálculo original. O LEIA_ME contém o dicionário completo.

Os arquivos não contêm transações bancárias nem uma implementação completa do sistema descrito. São registros primários fictícios do exercício. `entradas.csv` e `observacoes.csv` contêm recortes explicitamente identificados; não pretendem conter todas as entradas da população de um contador. A semente, quando informada, identifica o desenho sintético do cenário, não um executável ausente.

## 6. Limite da conclusão

As anotações não preservam hora de emissão, corte de treino e versão executada. O número pode ser recalculado, mas não há como determinar se a previsão usou informação posterior ao evento. Os dados recuperados também não cobrem a série original de 18 meses.

## 7. Continuidade e detalhamento técnico

Executar o registro da hora de emissão das previsões e o corte de treino para confrontar as variáveis disponíveis antes de cada noite.

As especificações descrevem o recorte sintético e não equivalem a um programa executável de produção.

## 8. Trecho recuperado do notebook

```python
# Rascunho sem saídas de execução e sem versão do conjunto de treino.
# volume_final só fica disponível depois da noite.
features = [volume_final, dia_da_semana]
# Alternativa anotada, sem prova de execução: usar volume_previsto_antes_da_noite.
previsao = modelo_nao_preservado.predict(features)
```
