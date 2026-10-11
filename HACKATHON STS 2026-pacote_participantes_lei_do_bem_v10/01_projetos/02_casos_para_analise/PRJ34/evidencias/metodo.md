# PRJ34 — Método e referência

Documento sintético. Data de corte: 2025-04-28.

## 1. Referência anterior

Inventário manual e grafo de frequência com janela fixa são comparadores. Chamadas de manutenção periódica aparecem como dependência persistente; tráfego real de baixa frequência pode desaparecer entre janelas.

Os catálogos/manuais citados neste documento são extratos fictícios definidos acima; não são documentos internos de instituição real. O trecho fornecido descreve as funções relevantes à comparação.

## 2. Mecanismo e hipótese

Ponderar chamada pela recorrência condicionada ao calendário de manutenção e à latência causal entre serviços. Manter aresta quando houver contribuição causal em duas janelas; rebaixar tráfego concentrado somente na janela de manutenção. A hipótese é separar dependência funcional e coincidência de agenda sem apagar serviços de baixa frequência.

## 3. Protocolo e critérios

140 serviços; 500 pares candidatos a relação, dos quais 18 têm mudança confirmada por roteiro independente, ao longo de seis janelas de dez minutos. Grafo de frequência detecta 12/18 mudanças e cria 14 vínculos falsos; ponderado detecta 16/18 e cria cinco. Os contadores de vínculos falsos usam os 500 pares candidatos como exposição, não os 140 serviços. Versionamento frequencia-F1-v1→ponderado-P2-v2 está registrado.

## 4. Parâmetros, versões e execução registrada

- semente: 202534
- servicos: 140
- janelas: 6
- duracao_janela_min: 10
- mudancas_referencia: 18

Versões localizadas: frequencia-F1-v1, ponderado-P2-v2. Configuração estruturada: `configuracao.json`. Datas e estados: `cronologia.csv`.

## 5. Leitura e reconstrução dos resultados

`medicoes.csv` é a fonte dos contadores por cenário e das medições numéricas entregues. `resultados.csv` é calculado a partir dela, não é uma segunda confirmação independente. Para contadores, somar numeradores e denominadores apenas dentro de um ensaio. Ensaios diferentes podem compartilhar a população. Em resultados.csv, base_de_calculo preserva o total de referência e descricao_base explica o que ele representa. Para média, mediana e diferença entre maior e menor valor, usar a operação indicada. Para percentil_95 de histograma, ordenar valores e localizar o posto teto(0,95 × soma dos pesos). valor_observado reproduz uma medição única. indicador_precalculado transcreve um indicador já calculado: conferir essa transcrição não reproduz seu cálculo original. O LEIA_ME contém o dicionário completo.

Os arquivos não contêm transações bancárias nem uma implementação completa do sistema descrito. São registros primários fictícios do exercício. `entradas.csv` e `observacoes.csv` contêm recortes explicitamente identificados; não pretendem conter todas as entradas da população de um contador. A semente, quando informada, identifica o desenho sintético do cenário, não um executável ausente.

## 6. Limite da conclusão

O critério de remoção de vínculos indevidos segue em teste: expiração em duas janelas elimina três falsos, mas também uma dependência real rara. Não há regra aprovada para esse compromisso, incluído na pretensão do mapa.

## 7. Continuidade e detalhamento técnico

Executar a comparação da expiração dos vínculos com a dependência real rara, distinguindo remoção de relação indevida e perda de relação válida.

As especificações descrevem o recorte sintético e não equivalem a um programa executável de produção.
