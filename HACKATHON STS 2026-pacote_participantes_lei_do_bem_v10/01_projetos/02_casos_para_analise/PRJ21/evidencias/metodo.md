# PRJ21 — Método e referência

Documento sintético. Data de corte: 2025-08-18.

## 1. Referência anterior

O runbook fictício GW-7 já descreve circuit breaker, limite de concorrência e fila por dependência. A configuração é escolhida dentro das faixas documentadas.

Os catálogos/manuais citados neste documento são extratos fictícios definidos acima; não são documentos internos de instituição real. O trecho fornecido descreve as funções relevantes à comparação.

## 2. Mecanismo e hipótese

Configurar três serviços com concorrência 20, 10 e 15; abertura após cinco falhas em dez requisições; reabertura de teste em 30 segundos. No serviço S2, ajustar abertura de três para cinco falhas porque o limiar inicial antecipava o bloqueio.

## 3. Protocolo e critérios

Nove roteiros, três por serviço: carga normal, lentidão e recuperação. config-v1 falhou no roteiro S2/normal; config-v2 cumpriu todos. O mecanismo de isolamento é o implementado no gateway.

## 4. Parâmetros, versões e execução registrada

- limiar_inicial: 3
- limiar_final: 5
- janela_chamadas: 10

Versões localizadas: config-v1, config-v2. Configuração estruturada: `configuracao.json`. Datas e estados: `cronologia.csv`.

## 5. Leitura e reconstrução dos resultados

`medicoes.csv` é a fonte dos contadores por cenário e das medições numéricas entregues. `resultados.csv` é calculado a partir dela, não é uma segunda confirmação independente. Para contadores, somar numeradores e denominadores apenas dentro de um ensaio. Ensaios diferentes podem compartilhar a população. Em resultados.csv, base_de_calculo preserva o total de referência e descricao_base explica o que ele representa. Para média, mediana e diferença entre maior e menor valor, usar a operação indicada. Para percentil_95 de histograma, ordenar valores e localizar o posto teto(0,95 × soma dos pesos). valor_observado reproduz uma medição única. indicador_precalculado transcreve um indicador já calculado: conferir essa transcrição não reproduz seu cálculo original. O LEIA_ME contém o dicionário completo.

Os arquivos não contêm transações bancárias nem uma implementação completa do sistema descrito. São registros primários fictícios do exercício. `entradas.csv` e `observacoes.csv` contêm recortes explicitamente identificados; não pretendem conter todas as entradas da população de um contador. A semente, quando informada, identifica o desenho sintético do cenário, não um executável ausente.

## 6. Limite da conclusão

Cobertura dos serviços e perfis de homologação. Não se propôs alterar o mecanismo de isolamento.

## 7. Continuidade e detalhamento técnico

Executar a aplicação dos perfis de homologação a novos serviços e conferir os parâmetros de isolamento fornecidos pela biblioteca.

As especificações descrevem o recorte sintético e não equivalem a um programa executável de produção.
