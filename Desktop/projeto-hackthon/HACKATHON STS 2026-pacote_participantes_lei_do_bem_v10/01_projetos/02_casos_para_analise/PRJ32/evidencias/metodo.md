# PRJ32 — Método e referência

Documento sintético. Data de corte: 2025-08-25.

## 1. Referência anterior

A minuta sugere reputação pelo histórico do dispositivo, mas os sinais estabilidade, confiança e contexto não têm definição mensurável. Não há política para aparelho novo.

Os catálogos/manuais citados neste documento são extratos fictícios definidos acima; não são documentos internos de instituição real. O trecho fornecido descreve as funções relevantes à comparação.

## 2. Mecanismo e hipótese

O diagrama apresenta coleta→pontuação→autenticação adicional. Um formulário registra opiniões de segurança sobre possíveis sinais; não especifica fórmula, atualização ou tratamento de ausência.

## 3. Protocolo e critérios

Oito fichas sintéticas de dispositivo foram entregues com campos modelo e versão de sistema. Os campos de reputação estão vazios porque a regra ainda não foi definida. Não há decisão de autenticação produzida a partir dessas fichas.

## 4. Parâmetros, versões e execução registrada

- sinais_definidos: false
- regra_aparelho_novo: null
- versao_modelo: null

Versões localizadas: dispositivo-v1. Configuração estruturada: `configuracao.json`. Datas e estados: `cronologia.csv`.

## 5. Leitura e reconstrução dos resultados

`medicoes.csv` é a fonte dos contadores por cenário e das medições numéricas entregues. `resultados.csv` é calculado a partir dela, não é uma segunda confirmação independente. Para contadores, somar numeradores e denominadores apenas dentro de um ensaio. Ensaios diferentes podem compartilhar a população. Em resultados.csv, base_de_calculo preserva o total de referência e descricao_base explica o que ele representa. Para média, mediana e diferença entre maior e menor valor, usar a operação indicada. Para percentil_95 de histograma, ordenar valores e localizar o posto teto(0,95 × soma dos pesos). valor_observado reproduz uma medição única. indicador_precalculado transcreve um indicador já calculado: conferir essa transcrição não reproduz seu cálculo original. O LEIA_ME contém o dicionário completo.

Os arquivos não contêm transações bancárias nem uma implementação completa do sistema descrito. São registros primários fictícios do exercício. `entradas.csv` e `observacoes.csv` contêm recortes explicitamente identificados; não pretendem conter todas as entradas da população de um contador. A semente, quando informada, identifica o desenho sintético do cenário, não um executável ausente.

## 6. Limite da conclusão

Faltam definição do mecanismo, critério de referência e ensaio. As fichas e o desenho demonstram preparação, não execução de técnica identificável.

## 7. Continuidade e detalhamento técnico

Executar a associação das fichas de dispositivo à regra de reputação e às decisões esperadas antes de comparar primeira aparição e recorrência.

As especificações descrevem o recorte sintético e não equivalem a um programa executável de produção.
