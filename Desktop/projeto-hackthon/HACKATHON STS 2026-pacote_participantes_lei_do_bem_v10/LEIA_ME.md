# Massa de projetos — Lei do Bem (exercício fictício)

Este pacote contém 40 projetos inteiramente fictícios: 20 históricos classificados e 20 casos para análise. Pessoas, instituições, sistemas, catálogos, eventos e medições foram criados para o exercício. Não são registros operacionais de um banco.

**Comece pelo `GUIA_DO_PARTICIPANTE.md`**, que explica o objetivo, as quatro classes e como conduzir a análise. Este arquivo descreve o conteúdo técnico do pacote.

## Objeto da decisão

Classificar o projeto pelo trabalho técnico comprovado nos arquivos entregues e justificar a conclusão com fontes. Não avaliar segregação de horas ou despesas, valores de incentivo ou requisitos fiscais da empresa. A massa não contém colunas de horas. Complexidade, valor para o negócio, uso de IA e quantidade de testes não determinam elegibilidade.

## Quatro classes

- **Elegível:** as evidências caracterizam P&D no escopo explicitamente definido: novidade técnica frente à referência anterior, criatividade, incerteza investigada, trabalho sistemático e conhecimento registrável ou transferível. Limites externos claramente excluídos não exigem ressalva automática; sucesso comercial ou ausência total de falhas não são requisitos.
- **Com ressalvas:** existe prova suficiente de P&D, mas uma limitação técnica ou de validação concreta restringe uma parte da conclusão pretendida dentro do escopo. Identificar o recorte sustentado, a limitação e a evidência necessária para resolver a ressalva. Não usar esta classe para preencher ausência essencial de prova.
- **Não elegível:** as evidências permitem caracterizar o trabalho como aplicação conhecida, configuração, integração, migração ou verificação rotineira, sem investigação tecnológica demonstrada. A conclusão decorre do mecanismo documentado, não do título.
- **Evidência insuficiente:** falta informação essencial para caracterizar ou verificar o núcleo alegado; não é possível concluir fundamentadamente entre P&D e rotina. Especificar o elo ausente, mesmo quando há planejamento, entradas parciais ou métricas de um componente.

Estas quatro classes são a taxonomia didática do exercício. O recorte é técnico, não uma concessão ou cálculo de benefício tributário.

## Evidências e arquivos

Cada projeto possui dossiê, registro técnico, entrevista, atividades em XLSX e CSV, inventário e arquivos auxiliares de método, configuração, cronologia, medições, resultados, entradas, observações e revisão. Os nomes e o número de arquivos foram uniformizados; o conteúdo e a suficiência variam.

`Localizada` no inventário significa que o arquivo está presente. Não significa que a alegação foi comprovada. Não presumir arquivos extras mencionados em conversa. O pacote explicita quando uma afirmação é somente memorando recuperado. Método e parâmetros concretos devem ser lidos; não basta contar anexos.

`medicoes.csv` contém registros numéricos primários fictícios: contadores por cenário, medições individuais ou histogramas discretos. `resultados.csv` consolida esse arquivo por cálculo ou transcrição, conforme a operação indicada. A repetição de números em PDFs não constitui confirmação independente. Os recortes de entradas e observações são identificados como recortes; não prometem todos os dados de uma população de milhares de casos.

Contagens de material entregue indicam a quantidade de linhas, fichas ou campos disponíveis, não o desempenho do mecanismo. Esses ensaios são identificados na métrica como contagem de entrega e têm `taxa_percentual` vazia; Os contadores originais ficam em `medicoes.csv`; em `resultados.csv`, `base_de_calculo` preserva o total. Uma contagem completa não comprova execução, qualidade ou sucesso técnico.

As datas e versões pertencem à narrativa fictícia. Não se afirma que os algoritmos bancários descritos foram executados na preparação deste pacote. A análise consiste em confrontar os artefatos efetivamente entregues, sua coerência e sua suficiência para classificação. Não é necessário implementar os sistemas descritos.

## Divergências entre fontes

Preservar divergências entre depoimento e documento na justificativa: registrar o que cada fonte afirma, o arquivo ou ensaio que sustenta cada afirmação e qual evidência fundamenta a conclusão. Verificar versão, escopo, unidade e denominador antes de comparar números.

Um resultado desfavorável pode pertencer a P&D; um aceite perfeito pode pertencer a rotina.

## Estrutura e formatos

- `00_dados_apoio`: índices consolidados, com os mesmos registros locais.
- `01_projetos/01_historico`: PRJ01–PRJ20.
- `01_projetos/02_casos_para_analise`: PRJ21–PRJ40, sem respostas ou dificuldade indicada.
- `historicos_classificados.csv` e `.md`: justificativas exclusivamente dos históricos.

CSVs: UTF-8 com BOM, separador ponto e vírgula, ponto decimal em campos numéricos. PDFs e narrativas usam convenção brasileira. Valores ausentes são vazios ou `null`, nunca zero implícito. Atividades XLSX: cabeçalho na linha 5. As atividades não têm horas ou despesas. Referências EV identificam arquivos; ensaios S identificam séries em medições e resultados.

## Confidencialidade

Os dados são fictícios, mas confidenciais para fins do evento e não podem sair do ambiente indicado pela organização. As regras administrativas do evento são fornecidas pela organização fora deste pacote.

## Dicionário de resultados.csv

Cada linha corresponde a um ensaio e uma versão. Leia a métrica, a operação e a base antes de interpretar o valor. Campos vazios não representam zero.

| Coluna              | Significado                                                                                                                                                                                                                            |
| ------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `ensaio_id`       | Identificador do ensaio. Liga o resultado às medições e à configuração.                                                                                                                                                          |
| `versao`          | Versão de entrega avaliada nesse ensaio.                                                                                                                                                                                              |
| `metrica`         | O que foi contado ou medido; indica se o valor se refere a acerto, falha, tempo ou outro indicador.                                                                                                                                    |
| `operacao`        | Como o valor foi obtido: contagem, media, mediana, diferenca_maior_menor, percentil_95, valor_observado ou indicador_precalculado.                                                                                                     |
| `valor`           | Resultado expresso na unidade indicada. Um valor maior pode representar melhora ou piora, conforme a métrica.                                                                                                                         |
| `base_de_calculo` | Total de referência das contagens; quantidade de valores nas médias, medianas e diferenças; soma das frequências no percentil; 1 para um valor observado ou indicador transcrito. Só funciona como divisor da taxa nas contagens. |
| `descricao_base`  | Explica o que a base representa neste ensaio: propostas, ataques, grupos, observações ou indicadores registrados.                                                                                                                    |
| `taxa_percentual` | Em contagem de desempenho: 100 × valor / base_de_calculo. Vazia nas demais operações e nas contagens de entrega. Vazio não significa zero.                                                                                         |
| `unidade`         | Unidade do valor: casos, s (segundos), ms (milissegundos), min (minutos), pontos percentuais ou escala 0-1. Pontos percentuais medem diferença entre percentuais.                                                                     |
| `fonte`           | Arquivo relativo à pasta do projeto e identificador do ensaio após #. O identificador seleciona as linhas desse ensaio no arquivo de medições do próprio projeto.                                                                 |
| `natureza`        | desempenho = indicador observado no ensaio, inclusive falhas; entrega = quantidade de material disponível. Não equivale à classificação do projeto.                                                                               |

### Operações em linguagem simples

| Operação                 | Como ler e conferir                                                                                                                                                              |
| -------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `contagem`               | Somar as ocorrências registradas no numerador das medições do mesmo ensaio. Somar seus denominadores para obter a base de cálculo.                                           |
| `media`                  | Somar os valores registrados e dividir pela quantidade de observações.                                                                                                         |
| `mediana`                | Ordenar os valores e obter o valor central; se houver quantidade par, usar a média dos dois valores centrais.                                                                   |
| `diferenca_maior_menor`  | Subtrair o menor valor do maior. A base informa quantos valores foram comparados; não é um divisor.                                                                            |
| `percentil_95`           | Ordenar os valores do histograma e acumular suas frequências até atingir o posto teto(0,95 × total de observações). Pelo menos 95% das observações ficam até esse valor. |
| `valor_observado`        | Reproduzir uma única medição registrada, sem calcular média nem presumir repetições.                                                                                       |
| `indicador_precalculado` | Transcrever um indicador previamente calculado e registrado. A conferência valida a transcrição; não refaz o cálculo original do indicador sem os dados necessários.       |

`base_de_calculo` não é sempre um divisor: na diferença entre taxas de quatro grupos, a base é 4 e a conta é maior menos menor. No percentil, a base é a soma das frequências do histograma. Para um indicador previamente calculado, base 1 significa um valor registrado, sem informar o tamanho da população original. A população e o protocolo devem ser consultados no método.

A porcentagem fica vazia em médias, medianas, diferenças, percentis e valores únicos, preservando a unidade do resultado. Percentual de falhas e percentual de acertos têm interpretações opostas. Uma contagem de entrega não comprova desempenho. A coluna natureza mantém os valores desempenho e entrega.

### Relação com medicoes.csv

O arquivo de medições mantém seu formato: `registro_id` identifica a linha; `ensaio_id` liga as linhas ao resultado; `versao` e `cenario` identificam a condição; `tipo` distingue contador, medicao e histograma; `metrica` diz o que foi observado; `valor` guarda a medição; `numerador` e `denominador` são os contadores originais; `peso` é a frequência no histograma; `unidade` qualifica o valor. Somar somente linhas do mesmo ensaio. Ensaios e versões podem compartilhar a mesma população. Não somar seus totais como pessoas ou propostas diferentes.

### Consulta de 00_dados_apoio

`projetos.csv` lista os 40 projetos, suas pastas e quantidades de atividades/evidências. `atividades_consolidadas.csv` reúne os registros dos arquivos locais de atividades. `evidencias.csv` reúne os inventários: sua coluna `arquivo` é relativa à pasta do projeto, identificada em projetos.csv. `documentos.csv` faz a ligação entre projeto, evidência e arquivo; `caminho_no_pacote` é relativo à raiz do pacote. Nas referências `arquivo#identificador`, o trecho após # identifica um ensaio ou seção, não outro arquivo.
