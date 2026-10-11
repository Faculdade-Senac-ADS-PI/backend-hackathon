# PRJ36 — Método e referência

Documento sintético. Data de corte: 2025-07-07.

## 1. Referência anterior

A plataforma fictícia OBS-8 oferece consultas, filtros, layout e acesso. O documento de demanda usa os termos nova arquitetura e observabilidade avançada para a troca de ferramenta.

Os catálogos/manuais citados neste documento são extratos fictícios definidos acima; não são documentos internos de instituição real. O trecho fornecido descreve as funções relevantes à comparação.

## 2. Mecanismo e hipótese

Traduzir consultas para sintaxe da plataforma mantendo filtros e agregação. Exemplo: sum(erros)/sum(chamadas) por janela de cinco minutos. O conversor de consulta e os widgets são do produto; as regras de alerta permanecem idênticas.

## 3. Protocolo e critérios

27 painéis × três verificações de valor, aparência e acesso, total 81. Quatro falhas iniciais de cor, filtro e permissão foram corrigidas. Um rascunho de painel foi contado na proposta de 28, mas não migrou por duplicar conteúdo.

## 4. Parâmetros, versões e execução registrada

- paineis_propostos: 28
- paineis_migrados: 27
- janela_min: 5

Versões localizadas: paineis-v1, paineis-v2. Configuração estruturada: `configuracao.json`. Datas e estados: `cronologia.csv`.

## 5. Leitura e reconstrução dos resultados

`medicoes.csv` é a fonte dos contadores por cenário e das medições numéricas entregues. `resultados.csv` é calculado a partir dela, não é uma segunda confirmação independente. Para contadores, somar numeradores e denominadores apenas dentro de um ensaio. Ensaios diferentes podem compartilhar a população. Em resultados.csv, base_de_calculo preserva o total de referência e descricao_base explica o que ele representa. Para média, mediana e diferença entre maior e menor valor, usar a operação indicada. Para percentil_95 de histograma, ordenar valores e localizar o posto teto(0,95 × soma dos pesos). valor_observado reproduz uma medição única. indicador_precalculado transcreve um indicador já calculado: conferir essa transcrição não reproduz seu cálculo original. O LEIA_ME contém o dicionário completo.

Os arquivos não contêm transações bancárias nem uma implementação completa do sistema descrito. São registros primários fictícios do exercício. `entradas.csv` e `observacoes.csv` contêm recortes explicitamente identificados; não pretendem conter todas as entradas da população de um contador. A semente, quando informada, identifica o desenho sintético do cenário, não um executável ausente.

## 6. Limite da conclusão

A arquitetura de coleta permanece a fornecida pela ferramenta. A mudança de sintaxe não corresponde a novo método de detecção.

## 7. Continuidade e detalhamento técnico

Executar a conferência dos painéis migrados após a próxima atualização de sintaxe da ferramenta de monitoramento.

As especificações descrevem o recorte sintético e não equivalem a um programa executável de produção.
