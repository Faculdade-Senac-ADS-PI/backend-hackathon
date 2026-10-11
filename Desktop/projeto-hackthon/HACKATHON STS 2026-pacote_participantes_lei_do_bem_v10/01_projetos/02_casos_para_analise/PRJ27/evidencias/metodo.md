# PRJ27 — Método e referência

Documento sintético. Data de corte: 2025-03-03.

## 1. Referência anterior

Busca lexical, busca semântica e geração com recuperação foram consideradas. O desenho propõe abstenção por suporte documental, além de localizar um trecho semelhante, mas a versão do gerador não foi preservada.

Os catálogos/manuais citados neste documento são extratos fictícios definidos acima; não são documentos internos de instituição real. O trecho fornecido descreve as funções relevantes à comparação.

## 2. Mecanismo e hipótese

Documento de arquitetura descreve recuperar três trechos, verificar se cada oração da resposta tem suporte e recusar se algum fato decisivo não tiver fonte. Há template de prompt e catálogo sintético de seis documentos. A entrega contém saídas do recuperador para 46 perguntas; não contém as respostas geradas nem julgamentos de suporte por oração.

## 3. Protocolo e critérios

Plano para 300 perguntas. Das 50 perguntas da demonstração citada no memorando, 46 textos foram recuperados, mas sem IDs comuns com a rodada de geração. Um teste separado do recuperador encontrou trecho relevante em 38 das 46 consultas. A apresentação atribuía 82% de respostas corretas em 50, sem decisões por pergunta.

## 4. Parâmetros, versões e execução registrada

- perguntas_planejadas: 300
- perguntas_recuperadas: 46
- top_k: 3
- versao_recuperador: "busca-B2-v1"
- versao_gerador: null
- template_prompt: "Responda com fonte por afirmação; recuse se não houver suporte."

Versões localizadas: busca-B2-v1. Configuração estruturada: `configuracao.json`. Datas e estados: `cronologia.csv`.

## 5. Leitura e reconstrução dos resultados

`medicoes.csv` é a fonte dos contadores por cenário e das medições numéricas entregues. `resultados.csv` é calculado a partir dela, não é uma segunda confirmação independente. Para contadores, somar numeradores e denominadores apenas dentro de um ensaio. Ensaios diferentes podem compartilhar a população. Em resultados.csv, base_de_calculo preserva o total de referência e descricao_base explica o que ele representa. Para média, mediana e diferença entre maior e menor valor, usar a operação indicada. Para percentil_95 de histograma, ordenar valores e localizar o posto teto(0,95 × soma dos pesos). valor_observado reproduz uma medição única. indicador_precalculado transcreve um indicador já calculado: conferir essa transcrição não reproduz seu cálculo original. O LEIA_ME contém o dicionário completo.

Os arquivos não contêm transações bancárias nem uma implementação completa do sistema descrito. São registros primários fictícios do exercício. `entradas.csv` e `observacoes.csv` contêm recortes explicitamente identificados; não pretendem conter todas as entradas da população de um contador. A semente, quando informada, identifica o desenho sintético do cenário, não um executável ausente.

O resultado de recuperação de trechos é 38/46, aproximadamente 82,6%, no teste busca-B2-v1. Os 82% (41/50) do memorando se referem a respostas. As unidades, populações e registros são diferentes, sem vínculo entre IDs; as duas medidas não são comparáveis e a recuperação de trechos não valida o desempenho das respostas.

## 6. Limite da conclusão

38/46 mede recuperação de trechos. A rodada de geração exigia também registrar a resposta, a fonte de cada afirmação e a decisão de recusa. Esses registros e a versão do gerador não foram recuperados após o encerramento do ambiente. Não há correspondência entre os IDs do teste busca-B2-v1 e os da demonstração mencionada no memorando.

## 7. Continuidade e detalhamento técnico

Executar a vinculação de cada pergunta ao trecho recuperado, à resposta gerada, à fonte de cada afirmação e à decisão de recusa.

As especificações descrevem o recorte sintético e não equivalem a um programa executável de produção.

## 8. Catálogo documental fictício recuperado

D01#p1: O prazo de compensação depende do canal e do horário; consultar o calendário da modalidade.
D02#p1: A segunda via pode ser emitida; encargos dependem das condições registradas no boleto.
D03#p1: Alterar favorecido exige cancelamento e nova emissão autorizada; não editar a identidade no boleto já emitido.
D04#p1: Se a fonte não sustenta a orientação, não inventar procedimento; encaminhar à área responsável.
D05#p1: A consulta retorna situação registrada, data de atualização e identificador fictício.
D06#p1: O cancelamento depende da situação do boleto; não afirmar que um boleto liquidado pode ser cancelado.
