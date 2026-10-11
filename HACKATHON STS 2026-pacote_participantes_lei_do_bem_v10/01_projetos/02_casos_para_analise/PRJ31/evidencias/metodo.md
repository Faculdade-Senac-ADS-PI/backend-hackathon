# PRJ31 — Método e referência

Documento sintético. Data de corte: 2025-07-21.

## 1. Referência anterior

Documento isolado, combinação fixa de documento+dispositivo e operação parcialmente offline foram comparados. A combinação fixa acumula sinais correlacionados como se fossem independentes, elevando confiança quando o mesmo artefato é reapresentado após reconexão.

Os catálogos/manuais citados neste documento são extratos fictícios definidos acima; não são documentos internos de instituição real. O trecho fornecido descreve as funções relevantes à comparação.

## 2. Mecanismo e hipótese

Representar evidência por origem, época, validade e dependência. Na retomada, reaproveitar somente sinais ainda válidos; limitar a contribuição conjunta de sinais derivados do mesmo documento e exigir um fator de origem independente. O protótipo condicionado-C3-v2 mantém um orçamento de risco condicionado à ausência, em vez de somar escores fixos. Reapresentação de sinal não adiciona confiança.

## 3. Protocolo e critérios

520 jornadas legítimas e 120 ataques, em quatro aparelhos × cinco redes, 20 células. condicionado-C3-v2 aprova 447 legítimas e bloqueia 116 ataques. Duzentas das 520 legítimas foram interrompidas; 182 retomaram (91%). Retomar significa continuar a avaliação, não aprovar identidade. Comparador fixo aprova 430 e bloqueia 108; os quatro ataques restantes de condicionado-C3-v2 estão em duas células de rede degradada.

## 4. Parâmetros, versões e execução registrada

- semente: 202531
- aparelhos: 4
- redes: 5
- interrompidas_legitimas: 200
- origens_minimas_independentes: 2

Versões localizadas: fixo-C1-v1, condicionado-C3-v2. Configuração estruturada: `configuracao.json`. Datas e estados: `cronologia.csv`.

## 5. Leitura e reconstrução dos resultados

`medicoes.csv` é a fonte dos contadores por cenário e das medições numéricas entregues. `resultados.csv` é calculado a partir dela, não é uma segunda confirmação independente. Para contadores, somar numeradores e denominadores apenas dentro de um ensaio. Ensaios diferentes podem compartilhar a população. Em resultados.csv, base_de_calculo preserva o total de referência e descricao_base explica o que ele representa. Para média, mediana e diferença entre maior e menor valor, usar a operação indicada. Para percentil_95 de histograma, ordenar valores e localizar o posto teto(0,95 × soma dos pesos). valor_observado reproduz uma medição única. indicador_precalculado transcreve um indicador já calculado: conferir essa transcrição não reproduz seu cálculo original. O LEIA_ME contém o dicionário completo.

Os arquivos não contêm transações bancárias nem uma implementação completa do sistema descrito. São registros primários fictícios do exercício. `entradas.csv` e `observacoes.csv` contêm recortes explicitamente identificados; não pretendem conter todas as entradas da população de um contador. A semente, quando informada, identifica o desenho sintético do cenário, não um executável ausente.

## 6. Limite da conclusão

A investigação mostra melhora, mas falha em quatro ataques com documento e dispositivo coerentemente comprometidos. A prova de fator independente não foi validada sob indisponibilidade prolongada da terceira origem, condição incluída na pretensão de recuperação remota.

## 7. Continuidade e detalhamento técnico

Executar a avaliação do fator independente nos ataques com documento e dispositivo comprometidos, incluindo indisponibilidade prolongada da terceira origem.

As especificações descrevem o recorte sintético e não equivalem a um programa executável de produção.
