# PRJ23 — Método e referência

Documento sintético. Data de corte: 2025-04-21.

## 1. Referência anterior

TTL fixo de 30 s e invalidação monotônica por versão escalar eram conhecidos. Uma versão escalar protege contra regressão da mesma origem, mas não representa confirmação parcial de duas origens que publicam a mesma transferência.

Os catálogos/manuais citados neste documento são extratos fictícios definidos acima; não são documentos internos de instituição real. O trecho fornecido descreve as funções relevantes à comparação.

## 2. Mecanismo e hipótese

Proposta: vetor (versão débito, versão crédito) associado à intenção. Só promover o saldo quando ambas as componentes dominam o vetor confirmado; evento dominado não invalida versão mais nova. Para componente pendente, consultar a confirmação uma vez por intenção e compartilhar a resposta entre leitores. A hipótese combina confirmação vetorial e coalescência de consulta para controlar defasagem sem serializar todas as leituras no núcleo.

## 3. Protocolo e critérios

30 mil leituras e quatro mil alterações em quatro perfis de atraso máximo de 0, 30, 90 e 180 s. As mesmas entradas alimentam TTL, versão escalar e confirmação vetorial. Critérios prévios: defasagem ≤ 0,3% e consultas extras ao núcleo ≤ 5% das leituras. Vetorial: 60 leituras defasadas (0,2%) e 900 consultas extras (3%). As 60 leituras residuais se distribuem em 12 no perfil de 90 s e 48 no de 180 s (PRJ23-S03).

## 4. Parâmetros, versões e execução registrada

- semente: 202523
- ttl_s: 30
- perfis_atraso_s: [0, 30, 90, 180]
- origens: ["debito", "credito"]
- limite_defasagem_pct: 0.3
- limite_consultas_pct: 5

Versões localizadas: ttl-v1, escalar-v2, vetorial-v3, entrada-v4. Configuração estruturada: `configuracao.json`. Datas e estados: `cronologia.csv`.

## 5. Leitura e reconstrução dos resultados

`medicoes.csv` é a fonte dos contadores por cenário e das medições numéricas entregues. `resultados.csv` é calculado a partir dela, não é uma segunda confirmação independente. Para contadores, somar numeradores e denominadores apenas dentro de um ensaio. Ensaios diferentes podem compartilhar a população. Em resultados.csv, base_de_calculo preserva o total de referência e descricao_base explica o que ele representa. Para média, mediana e diferença entre maior e menor valor, usar a operação indicada. Para percentil_95 de histograma, ordenar valores e localizar o posto teto(0,95 × soma dos pesos). valor_observado reproduz uma medição única. indicador_precalculado transcreve um indicador já calculado: conferir essa transcrição não reproduz seu cálculo original. O LEIA_ME contém o dicionário completo.

Os arquivos não contêm transações bancárias nem uma implementação completa do sistema descrito. São registros primários fictícios do exercício. `entradas.csv` e `observacoes.csv` contêm recortes explicitamente identificados; não pretendem conter todas as entradas da população de um contador. A semente, quando informada, identifica o desenho sintético do cenário, não um executável ausente.

## 6. Limite da conclusão

Escopo pré-definido: até 180 s e uma origem temporariamente indisponível. Duas origens indisponíveis simultaneamente e atrasos superiores foram excluídos antes dos ensaios; não são condições incluídas na conclusão. São 60 leituras residuais, 12 no perfil de 90 s e 48 no de 180 s; a janela está documentada em PRJ23-S03.

## 7. Continuidade e detalhamento técnico

Executar o exame dos timestamps das leituras residuais nos perfis de 90 s e 180 s junto da confirmação das duas componentes.

As especificações descrevem o recorte sintético e não equivalem a um programa executável de produção.

```text
ao receber componente de uma intencao:
  se componente e dominada pelo vetor_confirmado: ignorar
  registrar_componente_pendente(intencao, origem, versao)
  se ambas_origens_confirmam: promover_vetor_e_saldo_atomicamente()
ao ler intencao pendente:
  compartilhar_uma_consulta_de_confirmacao_por_intencao()
  se ainda sem confirmacao: registrar_versao_servida_e_defasagem()
```


