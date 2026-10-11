# PRJ30 — Método e referência

Documento sintético. Data de corte: 2025-06-16.

## 1. Referência anterior

O produto fictício MFA-6 fornece SDK, OTP, notificação, recuperação e roteiro de contingência. Parâmetros de validade, reenvio e fatores por perfil estão documentados.

Os catálogos/manuais citados neste documento são extratos fictícios definidos acima; não são documentos internos de instituição real. O trecho fornecido descreve as funções relevantes à comparação.

## 2. Mecanismo e hipótese

Integrar SDK e API; ajustar validade de 30 para 60 s, reenvio de 10 para 20 s, tolerância de relógio de 15 para 30 s e tentativas de três para cinco. Renovar certificado de homologação simbólico CERT-TESTE-02. Não modificar protocolo de autenticação.

## 3. Protocolo e critérios

72 casos de aceite em três canais. Cinco falhas iniciais estavam ligadas aos quatro parâmetros e ao certificado. Configuração final aprovada em todos os 72.

## 4. Parâmetros, versões e execução registrada

- ajustes_parametro: 4
- certificado: "CERT-TESTE-02; identificador fictício sem chave"

Versões localizadas: mfa-v1, mfa-v2. Configuração estruturada: `configuracao.json`. Datas e estados: `cronologia.csv`.

## 5. Leitura e reconstrução dos resultados

`medicoes.csv` é a fonte dos contadores por cenário e das medições numéricas entregues. `resultados.csv` é calculado a partir dela, não é uma segunda confirmação independente. Para contadores, somar numeradores e denominadores apenas dentro de um ensaio. Ensaios diferentes podem compartilhar a população. Em resultados.csv, base_de_calculo preserva o total de referência e descricao_base explica o que ele representa. Para média, mediana e diferença entre maior e menor valor, usar a operação indicada. Para percentil_95 de histograma, ordenar valores e localizar o posto teto(0,95 × soma dos pesos). valor_observado reproduz uma medição única. indicador_precalculado transcreve um indicador já calculado: conferir essa transcrição não reproduz seu cálculo original. O LEIA_ME contém o dicionário completo.

Os arquivos não contêm transações bancárias nem uma implementação completa do sistema descrito. São registros primários fictícios do exercício. `entradas.csv` e `observacoes.csv` contêm recortes explicitamente identificados; não pretendem conter todas as entradas da população de um contador. A semente, quando informada, identifica o desenho sintético do cenário, não um executável ausente.

## 6. Limite da conclusão

O exercício de contingência verifica comportamento contratado. Não se propôs mecanismo novo de autenticação.

## 7. Continuidade e detalhamento técnico

Executar a repetição do exercício de contingência do produto multifator após alteração dos canais de autenticação contratados.

As especificações descrevem o recorte sintético e não equivalem a um programa executável de produção.
