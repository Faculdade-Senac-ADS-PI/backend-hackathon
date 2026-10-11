# PRJ24 — Método e referência

Documento sintético. Data de corte: 2025-05-26.

## 1. Referência anterior

Injeção manual e scripts com temporizadores independentes não fixam a ordem parcial de falha e recuperação. Congelar o serviço para repetir a ordem muda o comportamento observado.

Os catálogos/manuais citados neste documento são extratos fictícios definidos acima; não são documentos internos de instituição real. O trecho fornecido descreve as funções relevantes à comparação.

## 2. Mecanismo e hipótese

Controlar somente o despachante de falhas por barreiras causais. Um evento de falha é liberado quando seus predecessores observáveis foram confirmados; relógios dos serviços seguem livres. O coletor atribui (origem, sequência, época) antes de agregar, preservando simultaneidade em vez de ordenar por chegada. A primeira revisão do coletor perdia 11 empates; a revisão causal-K2-v4 usa esse identificador estável.

## 3. Protocolo e critérios

32 sequências de até sete eventos, 20 repetições cada. Sequências S01–S11 usam L1; S12–S22 usam L2; S23–S32 usam L3: 220+220+200=640, sem produto cartesiano adicional. Manual, scripts e causal causal-K1-v3/causal-K2-v4 usam a mesma distribuição. Causal causal-K1-v3 preserva 629 e causal-K2-v4 preserva 640 ordens parciais e estados terminais.

## 4. Parâmetros, versões e execução registrada

- semente: 202524
- sequencias: 32
- repeticoes: 20
- matriz_perfis: {"S01-S11": 10, "S12-S22": 50, "S23-S32": 150}
- unidade_atraso: "ms"
- eventos_max: 7

Versões localizadas: manual-v1, scripts-v2, causal-K1-v3, causal-K2-v4. Configuração estruturada: `configuracao.json`. Datas e estados: `cronologia.csv`.

## 5. Leitura e reconstrução dos resultados

`medicoes.csv` é a fonte dos contadores por cenário e das medições numéricas entregues. `resultados.csv` é calculado a partir dela, não é uma segunda confirmação independente. Para contadores, somar numeradores e denominadores apenas dentro de um ensaio. Ensaios diferentes podem compartilhar a população. Em resultados.csv, base_de_calculo preserva o total de referência e descricao_base explica o que ele representa. Para média, mediana e diferença entre maior e menor valor, usar a operação indicada. Para percentil_95 de histograma, ordenar valores e localizar o posto teto(0,95 × soma dos pesos). valor_observado reproduz uma medição única. indicador_precalculado transcreve um indicador já calculado: conferir essa transcrição não reproduz seu cálculo original. O LEIA_ME contém o dicionário completo.

Os arquivos não contêm transações bancárias nem uma implementação completa do sistema descrito. São registros primários fictícios do exercício. `entradas.csv` e `observacoes.csv` contêm recortes explicitamente identificados; não pretendem conter todas as entradas da população de um contador. A semente, quando informada, identifica o desenho sintético do cenário, não um executável ausente.

## 6. Limite da conclusão

Conclusão restrita às 32 sequências, até sete eventos e perfis L1–L3. Grafo, pseudocódigo, matriz, sementes e rastros acompanham o pacote; não se promete um ambiente bancário executável.

## 7. Continuidade e detalhamento técnico

Executar a inclusão de sequências ao grafo mantendo as dependências causais e comparar repetições com a referência de estado terminal.

As especificações descrevem o recorte sintético e não equivalem a um programa executável de produção.

```text
para sequencia S da configuracao e repeticao1..20:
  restaurar_estado_inicial(); usar_semente_da_sequencia()
  liberar_falha_somente_quando_predecessores_foram_observados()
  manter_relogios_dos_servicos_livres()
  causal-K2-v4: identificar_evento_por(origem, sequencia, epoca)
  comparar_ordem_parcial_e_estado_terminal_com_referencia()
```



`configuracao.json` entrega os 32 grafos com eventos, predecessores, atrasos e sementes. A ordem parcial é dada pelas arestas; eventos sem caminho causal entre si podem ocorrer em qualquer ordem sem constituir divergência.
