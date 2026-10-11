# PRJ28 — Método e referência

Documento sintético. Data de corte: 2025-04-07.

## 1. Referência anterior

O planejamento chama a migração de nova abordagem por lotes. Não registra se o conversor era desenvolvido internamente ou recurso contratado, nem o limite do processo anterior.

Os catálogos/manuais citados neste documento são extratos fictícios definidos acima; não são documentos internos de instituição real. O trecho fornecido descreve as funções relevantes à comparação.

## 2. Mecanismo e hipótese

Minuta mapeia código, vigência e canal entre dois esquemas. Propõe converter em lotes e conferir emissão antes de liberar, mas não fixa tamanho, critério de parada ou mecanismo de continuidade.

## 3. Protocolo e critérios

Dez linhas de entrada de exemplo, duas correspondências de campo e cronograma foram entregues. Não há saída de conversão, versão do conversor ou registro de emissão durante a migração.

## 4. Parâmetros, versões e execução registrada

- tamanho_lote: null
- versao_conversor: null
- criterio_parada: null

Versões localizadas: convenio-v1. Configuração estruturada: `configuracao.json`. Datas e estados: `cronologia.csv`.

## 5. Leitura e reconstrução dos resultados

`medicoes.csv` é a fonte dos contadores por cenário e das medições numéricas entregues. `resultados.csv` é calculado a partir dela, não é uma segunda confirmação independente. Para contadores, somar numeradores e denominadores apenas dentro de um ensaio. Ensaios diferentes podem compartilhar a população. Em resultados.csv, base_de_calculo preserva o total de referência e descricao_base explica o que ele representa. Para média, mediana e diferença entre maior e menor valor, usar a operação indicada. Para percentil_95 de histograma, ordenar valores e localizar o posto teto(0,95 × soma dos pesos). valor_observado reproduz uma medição única. indicador_precalculado transcreve um indicador já calculado: conferir essa transcrição não reproduz seu cálculo original. O LEIA_ME contém o dicionário completo.

Os arquivos não contêm transações bancárias nem uma implementação completa do sistema descrito. São registros primários fictícios do exercício. `entradas.csv` e `observacoes.csv` contêm recortes explicitamente identificados; não pretendem conter todas as entradas da população de um contador. A semente, quando informada, identifica o desenho sintético do cenário, não um executável ausente.

## 6. Limite da conclusão

Não foram recuperados a versão do conversor, a origem do mecanismo, a comparação com o processo anterior ou os registros de execução durante emissão concorrente.

## 7. Continuidade e detalhamento técnico

Executar o acompanhamento de uma conversão de convênio durante emissão concorrente, identificando a versão do conversor e a equivalência dos registros.

As especificações descrevem o recorte sintético e não equivalem a um programa executável de produção.
