# PRJ26 — Método e referência

Documento sintético. Data de corte: 2025-08-04.

## 1. Referência anterior

O módulo fictício REP-4 oferece filtros parametrizados, colunas calculadas por expressões aritméticas, exportação e perfis. O catálogo chama o recurso de relatório inteligente, mas a fórmula é definida pelo usuário.

Os catálogos/manuais citados neste documento são extratos fictícios definidos acima; não são documentos internos de instituição real. O trecho fornecido descreve as funções relevantes à comparação.

## 2. Mecanismo e hipótese

Configurar 18 relatórios. Duas expressões parecem desenvolvimento: atraso=max(0,data_corte-vencimento) e saldo=principal-pago. Ambas usam funções listadas no manual; não há ajuste de modelo ou inferência. Dois layouts de um catálogo inicial de 20 foram descartados.

## 3. Protocolo e critérios

Cada relatório tem três testes: cálculo, filtro e permissão, total 54 verificações. Três falhas de permissão na primeira rodada; 54 resultados conformes na final. R01–R18 são relatórios distintos; alternativas de layout não são relatórios adicionais.

## 4. Parâmetros, versões e execução registrada

- catalogo_layouts: 20
- selecionados: 18
- formulas: ["MAX(0,data_corte-vencimento)", "principal-pago"]

Versões localizadas: rel-v1, rel-v2. Configuração estruturada: `configuracao.json`. Datas e estados: `cronologia.csv`.

## 5. Leitura e reconstrução dos resultados

`medicoes.csv` é a fonte dos contadores por cenário e das medições numéricas entregues. `resultados.csv` é calculado a partir dela, não é uma segunda confirmação independente. Para contadores, somar numeradores e denominadores apenas dentro de um ensaio. Ensaios diferentes podem compartilhar a população. Em resultados.csv, base_de_calculo preserva o total de referência e descricao_base explica o que ele representa. Para média, mediana e diferença entre maior e menor valor, usar a operação indicada. Para percentil_95 de histograma, ordenar valores e localizar o posto teto(0,95 × soma dos pesos). valor_observado reproduz uma medição única. indicador_precalculado transcreve um indicador já calculado: conferir essa transcrição não reproduz seu cálculo original. O LEIA_ME contém o dicionário completo.

Os arquivos não contêm transações bancárias nem uma implementação completa do sistema descrito. São registros primários fictícios do exercício. `entradas.csv` e `observacoes.csv` contêm recortes explicitamente identificados; não pretendem conter todas as entradas da população de um contador. A semente, quando informada, identifica o desenho sintético do cenário, não um executável ausente.

## 6. Limite da conclusão

A terminologia de protótipo inteligente da solicitação comercial não descreve o mecanismo efetivamente implementado.

## 7. Continuidade e detalhamento técnico

Executar a conferência dos relatórios solicitados no portal contra os filtros e agregações da ferramenta utilizada.

As especificações descrevem o recorte sintético e não equivalem a um programa executável de produção.
