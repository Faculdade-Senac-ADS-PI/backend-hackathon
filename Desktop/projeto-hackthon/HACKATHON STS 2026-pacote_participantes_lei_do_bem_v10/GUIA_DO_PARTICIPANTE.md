# Guia do participante

Leia este guia primeiro. Ele explica **o que classificar e como pensar a análise**. O `LEIA_ME.md` complementa com a descrição técnica dos arquivos e dos formatos.

## Objetivo

Esta massa contém 40 projetos fictícios de uma instituição financeira brasileira: 20 históricos classificados (PRJ01–PRJ20) e 20 casos para análise (PRJ21–PRJ40). A tarefa é produzir uma **recomendação preliminar, rastreável e defensável** a partir das evidências de cada projeto.

A ferramenta apoia o analista; ela não decide por ele. A conclusão final é do analista, e a solução deve deixar claro o caminho que levou até ela.

## Regras de uso dos dados

- Todos os nomes, dados, fornecedores, valores e eventos desta massa são fictícios. **É permitido e incentivado utilizar APIs externas de LLMs para processar os arquivos disponibilizados e apoiar a análise durante o desafio.** Essa autorização se aplica exclusivamente à massa fictícia fornecida pela organização e à finalidade do hackathon; não se estende a dados reais do banco ou de terceiros.
- Se a solução tratar dados de pessoas, deve respeitar a LGPD.
- Não há acesso aos sistemas do banco. Trabalhe só com os arquivos entregues.
- As regras administrativas do evento (prazos, critérios da banca, propriedade intelectual) constam do edital e do Guia do Desafio.

## Escopo da Lei do Bem no desafio

As equipes podem utilizar APIs externas de LLMs, modelos executados localmente ou uma combinação dessas abordagens. O uso de APIs externas é permitido e incentivado, mas não é obrigatório.

Independentemente da tecnologia escolhida, a solução deve fundamentar suas recomendações nas evidências do projeto, identificar as fontes utilizadas e permitir revisão pelo analista. A resposta do modelo não substitui a decisão humana.


## Escopo da Lei do Bem no desafio

Use uma visão abrangente das atividades de pesquisa tecnológica e desenvolvimento de inovação tecnológica. Pesquisa básica dirigida, pesquisa aplicada e desenvolvimento experimental podem ser relevantes conforme os fatos. O Guia do Desafio resume essa lógica em linguagem simples, centrada em desenvolvimento experimental; aqui a análise usa o conceito completo. Tecnologia moderna, grande esforço, desenvolvimento de software ou inovação para a empresa não bastam, por si sós, para demonstrar P&D.

A empresa usufrui o incentivo sem aprovação prévia, desde que cumpra os requisitos legais, mantenha os controles necessários e posteriormente preste as informações exigidas. A Lei nº 11.196/2005 é regulamentada pelo Decreto nº 5.798/2006. A IN RFB nº 1.187/2011 é uma instrução normativa da Receita Federal do Brasil.

## Objeto da decisão

Classificar o projeto pelo **trabalho técnico comprovado nos arquivos entregues** e justificar a conclusão com fontes.

Esta massa **não** pede:

- segregação de horas ou despesas (a massa não tem colunas de horas nem de gastos);
- cálculo de valor do incentivo;
- avaliação de requisitos fiscais da empresa.

Complexidade, valor para o negócio, uso de inteligência artificial e quantidade de testes **não determinam** elegibilidade.

## Critérios de análise

Avalie novidade, criatividade, incerteza tecnológica, sistematicidade e transferibilidade ou reprodutibilidade (os cinco critérios do Manual de Frascati). Use experimentação, conhecimento gerado, estado da técnica e distinção entre P&D e rotina como verificações complementares.

A coluna `natureza_informada_pela_equipe` é autodeclaração da equipe, não conclusão do avaliador. Nesta massa ela tem a mesma distribuição em todos os projetos e não ajuda a distinguir classes.

## Quatro classificações

### Elegível

As evidências caracterizam P&D no escopo explicitamente definido: novidade técnica frente à referência anterior, criatividade, incerteza investigada, trabalho sistemático e conhecimento registrável ou transferível.

- Limites externos claramente excluídos desde o início não exigem ressalva automática.
- Sucesso comercial e ausência total de falhas não são requisitos.

### Com ressalvas

Existe prova suficiente de P&D, mas uma **limitação técnica ou de validação concreta** restringe parte da conclusão pretendida, dentro do escopo.

Ao usar esta classe, indique:

1. o recorte sustentado pelas evidências;
2. a limitação específica;
3. a evidência necessária para resolver a ressalva.

Não use esta classe para preencher ausência essencial de prova: isso é Evidência insuficiente.

### Não elegível

As evidências permitem caracterizar o trabalho como aplicação de técnica conhecida, configuração, integração, migração, manutenção, aceite ou verificação rotineira, sem investigação tecnológica demonstrada. A conclusão decorre do **mecanismo documentado**, não do título do projeto.

### Evidência insuficiente

Falta informação essencial para caracterizar ou verificar o núcleo alegado, e não é possível concluir com fundamento entre P&D e rotina. Podem existir alegações, planos, arquitetura, entradas parciais ou métricas de um componente.

Especifique o **elo ausente** e as evidências adicionais a solicitar.

Não elegível não é o mesmo que Evidência insuficiente: no primeiro caso existe base para uma conclusão negativa; no segundo, a base é incompleta.

## Tratamento das evidências

Não considere uma afirmação verdadeira apenas porque aparece em entrevista, apresentação, dossiê ou declaração da equipe. Sempre que possível:

- relacione a afirmação ao arquivo ou registro que a sustenta (por ID);
- verifique versões, datas, entradas, parâmetros, unidades e denominadores;
- diferencie intenção ou planejamento de execução efetivamente registrada;
- registre informações favoráveis e contrárias à conclusão;
- registre contradições entre fontes.

Depoimentos e entrevistas dão contexto, mas afirmações de memória devem ser confrontadas com os registros.

**Quando duas fontes divergirem, não esconda nem resolva automaticamente a divergência.** Registre-a e indique qual evidência sustenta a conclusão adotada. Formato sugerido: "A entrevista afirma X; o registro Y (arquivo, ensaio ou ID) mostra Z; prevalece o registro, por ser primário e identificado por versão."

Um resultado desfavorável pode pertencer a P&D. Um aceite perfeito pode pertencer a rotina.

## Como ler os dados

- **`medicoes.csv`** traz os registros numéricos primários fictícios (contadores por cenário, medições individuais ou histogramas discretos). **`resultados.csv`** é derivado dele. Repetir um número em um PDF não é confirmação independente.
- **Contagens de entrega** (a métrica traz "contagem de entrega, não desempenho") indicam quantas linhas, fichas ou campos foram disponibilizados. Não medem o desempenho do mecanismo, e a taxa fica vazia. Uma contagem completa não comprova execução, qualidade ou sucesso técnico.
- **`Localizada`** no inventário significa apenas que o arquivo está presente. Não significa que a alegação foi comprovada.
- **`entradas.csv` e `observacoes.csv`** são recortes identificados como tais. Não representam toda a população de casos.
- **`resultado_ou_saida`** traz só indicadores ligados explicitamente à atividade. Se estiver vazio, o pacote não individualiza uma saída naquela linha. Isso não demonstra que o trabalho não ocorreu nem que a atividade é inelegível; confira o dossiê e os arquivos citados.
- A existência de um resultado registrado não demonstra, isoladamente, que houve P&D. Interprete-o junto do problema técnico, do estado anterior, da incerteza, do método e das demais evidências.
- Não presuma arquivos extras mencionados em conversa. Quando uma afirmação é só memorando recuperado, o pacote diz isso.

## Uso dos projetos históricos

PRJ01–PRJ20 são **referência de calibração**: mostram os critérios e o nível de fundamentação esperado. As classificações e justificativas estão em `historicos_classificados.csv` e `historicos_classificados.md`.

Não classifique por semelhança ("este projeto parece com o PRJ05, então recebe a mesma classe"). A classificação deve decorrer das **evidências do próprio projeto analisado**.

## Ordem de leitura sugerida

1. Leia o Guia do Desafio, este guia e o `LEIA_ME.md`.
2. Em cada projeto, leia o dossiê e identifique o problema, o estado anterior e o que a equipe fez.
3. Consulte as atividades e trate a natureza informada como autodeclaração.
4. Relacione cada afirmação às evidências por ID.
5. Separe falha operacional de falha experimental.
6. Diferencie novidade para a instituição de progresso tecnológico.
7. Confronte entrevista e documentos e registre as divergências.
8. Verifique método, versões, escopo, unidade e denominador antes de aceitar um resultado.
9. Registre evidências favoráveis, contrárias, ausentes e contraditórias.
10. Só então produza a classificação e a justificativa.

## Entrega esperada

Para os projetos PRJ21 a PRJ40, a solução deve ser capaz de produzir:

- classificação recomendada;
- avaliação dos cinco critérios;
- recorte sustentado, limitação e evidência necessária (quando Com ressalvas);
- elo ausente e evidências adicionais a solicitar (quando Evidência insuficiente);
- evidências utilizadas, por ID;
- evidências contrárias ou contraditórias, incluindo divergências entre depoimento e documento;
- lacunas identificadas;
- justificativa da conclusão;
- registro de rastreabilidade: regra ou critério aplicado, informação do projeto que sustenta a decisão, e quem decidiu e quando (a decisão final é do analista).


Durante a demonstração, é recomendado que a equipe esteja preparada para executar o fluxo de análise de ponta a ponta em pelo menos um caso: receber ou selecionar documentos, processar as evidências e gerar uma recomendação e um parecer sob demanda, permitindo revisão pelo analista. Não é necessário apresentar manualmente os 20 casos.

Resultados previamente gerados podem complementar a apresentação, mas não substituem a demonstração do processamento pela solução. São diferenciais incentivados: entrada de casos novos, upload de documentos, reanálise após inclusão de evidências e processamento em lote com acompanhamento do andamento.
