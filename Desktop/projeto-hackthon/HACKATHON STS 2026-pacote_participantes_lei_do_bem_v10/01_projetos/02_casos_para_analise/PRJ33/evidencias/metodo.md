# PRJ33 — Método e referência

Documento sintético. Data de corte: 2025-03-24.

## 1. Referência anterior

O catálogo descreve métricas, logs e rastros nativos. A equipe propõe ajustar relógios a partir de eventos correlatos, mas não entrega a especificação desse ajuste.

Os catálogos/manuais citados neste documento são extratos fictícios definidos acima; não são documentos internos de instituição real. O trecho fornecido descreve as funções relevantes à comparação.

## 2. Mecanismo e hipótese

Desenho conecta três agentes a um normalizador. Há um mapa de campos timestamp, host e correlation_id. Não define janela de associação nem estimação do desvio de relógio.

## 3. Protocolo e critérios

Doze eventos de exemplo foram recuperados de duas origens, com relógios diferentes. Quatro têm correlation_id. Nenhum tem timestamp corrigido ou vínculo produzido pelo normalizador. O memorando menciona prova de conceito sem identificar sua versão.

## 4. Parâmetros, versões e execução registrada

- versao_normalizador: null
- janela_associacao_s: null

Versões localizadas: eventos-v1. Configuração estruturada: `configuracao.json`. Datas e estados: `cronologia.csv`.

## 5. Leitura e reconstrução dos resultados

`medicoes.csv` é a fonte dos contadores por cenário e das medições numéricas entregues. `resultados.csv` é calculado a partir dela, não é uma segunda confirmação independente. Para contadores, somar numeradores e denominadores apenas dentro de um ensaio. Ensaios diferentes podem compartilhar a população. Em resultados.csv, base_de_calculo preserva o total de referência e descricao_base explica o que ele representa. Para média, mediana e diferença entre maior e menor valor, usar a operação indicada. Para percentil_95 de histograma, ordenar valores e localizar o posto teto(0,95 × soma dos pesos). valor_observado reproduz uma medição única. indicador_precalculado transcreve um indicador já calculado: conferir essa transcrição não reproduz seu cálculo original. O LEIA_ME contém o dicionário completo.

Os arquivos não contêm transações bancárias nem uma implementação completa do sistema descrito. São registros primários fictícios do exercício. `entradas.csv` e `observacoes.csv` contêm recortes explicitamente identificados; não pretendem conter todas as entradas da população de um contador. A semente, quando informada, identifica o desenho sintético do cenário, não um executável ausente.

## 6. Limite da conclusão

Sem saídas corrigidas, referência temporal e critério, os exemplos de entrada não demonstram a técnica de alinhamento alegada.

## 7. Continuidade e detalhamento técnico

Executar a definição de uma referência temporal para as origens A e B e registrar o timestamp corrigido junto do correlation_id.

As especificações descrevem o recorte sintético e não equivalem a um programa executável de produção.
