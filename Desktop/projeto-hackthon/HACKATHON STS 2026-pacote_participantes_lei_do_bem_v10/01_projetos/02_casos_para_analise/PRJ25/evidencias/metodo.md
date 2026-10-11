# PRJ25 — Método e referência

Documento sintético. Data de corte: 2025-06-30.

## 1. Referência anterior

Catálogo fictício COB-3 contém perfis, prazos, canais, modelos de mensagem e API de integração. A política de cobrança já estava aprovada antes da configuração.

Os catálogos/manuais citados neste documento são extratos fictícios definidos acima; não são documentos internos de instituição real. O trecho fornecido descreve as funções relevantes à comparação.

## 2. Mecanismo e hipótese

Selecionar componentes do catálogo e preencher prazo, canal e texto. Mapear perfil operador/supervisor a permissões existentes. Não alterar lógica da plataforma.

## 3. Protocolo e critérios

86 itens funcionais e 24 cenários de permissão aprovados na configuração final. Quatro itens propostos inicialmente foram retirados do escopo antes do aceite, conforme registro de revisão; não compõem os 86.

## 4. Parâmetros, versões e execução registrada

- itens_propostos: 90
- itens_retirados_antes_aceite: 4

Versões localizadas: config-v1. Configuração estruturada: `configuracao.json`. Datas e estados: `cronologia.csv`.

## 5. Leitura e reconstrução dos resultados

`medicoes.csv` é a fonte dos contadores por cenário e das medições numéricas entregues. `resultados.csv` é calculado a partir dela, não é uma segunda confirmação independente. Para contadores, somar numeradores e denominadores apenas dentro de um ensaio. Ensaios diferentes podem compartilhar a população. Em resultados.csv, base_de_calculo preserva o total de referência e descricao_base explica o que ele representa. Para média, mediana e diferença entre maior e menor valor, usar a operação indicada. Para percentil_95 de histograma, ordenar valores e localizar o posto teto(0,95 × soma dos pesos). valor_observado reproduz uma medição única. indicador_precalculado transcreve um indicador já calculado: conferir essa transcrição não reproduz seu cálculo original. O LEIA_ME contém o dicionário completo.

Os arquivos não contêm transações bancárias nem uma implementação completa do sistema descrito. São registros primários fictícios do exercício. `entradas.csv` e `observacoes.csv` contêm recortes explicitamente identificados; não pretendem conter todas as entradas da população de um contador. A semente, quando informada, identifica o desenho sintético do cenário, não um executável ausente.

## 6. Limite da conclusão

Aceite do comportamento contratado. Não há comparação de método novo ou obstáculo tecnológico investigado.

## 7. Continuidade e detalhamento técnico

Executar a homologação das próximas jornadas de cobrança com os mesmos controles de permissão da plataforma adquirida.

As especificações descrevem o recorte sintético e não equivalem a um programa executável de produção.
