# PRJ38 — Método e referência

Documento sintético. Data de corte: 2025-03-10.

## 1. Referência anterior

Execução sequencial-v4, futuros assíncronos e DAG estático eram conhecidos. Nos protótipos assíncronos, resposta rápida podia fixar aprovação antes de uma regra prioritária ou incorporar outra versão de cadastro.

Os catálogos/manuais citados neste documento são extratos fictícios definidos acima; não são documentos internos de instituição real. O trecho fornecido descreve as funções relevantes à comparação.

## 2. Mecanismo e hipótese

Fixar época de cadastro por proposta; avaliar nós independentes em paralelo; agregar resultados pela precedência do catálogo, não pela ordem de chegada. Cancelar ramo somente quando uma decisão dominante está provada e suas dependências fechadas. Persistir IDs das regras realmente consumidas e snapshot. Em timeout de dependência obrigatória, retornar revisão conforme mesma regra do motor sequencial-v4. motor-P1-v1 usava ordem de chegada; motor-P2-v2 fixava precedência mas misturava versões; motor-P3-v3 aplica ambas as barreiras.

## 3. Protocolo e critérios

148 regras e 240 mil propostas compartilhadas entre sequencial-v4 e motor-P1-v1/motor-P2-v2/motor-P3-v3. Matriz completa: 18 perfis de atraso × 5 falhas × 3 épocas = 270 células. São 888 propostas por célula e uma extra nas primeiras 240, totalizando 240.000. Cada proposta passa pelas quatro versões; não são 960.000 propostas distintas. motor-P1-v1 diverge em 96 decisões, motor-P2-v2 em 18 e motor-P3-v3 em zero. Os histogramas do sequencial-v4 e de motor-P3-v3 contêm 240 mil observações cada. Pelo posto teto(0,95 × n), o p95 passa de 2,84 para 1,42 segundo.

## 4. Parâmetros, versões e execução registrada

- semente: 202538
- regras: 148
- perfis_atraso: 18
- falhas: ["timeout", "resposta_duplicada", "fora_de_ordem", "queda_origem", "retorno_epoca_divergente"]
- epocas: [1, 2, 3]
- celulas: 270
- regra_quantile: "nearest rank"

Versões localizadas: motor-P1-v1, motor-P2-v2, motor-P3-v3, sequencial-v4. Configuração estruturada: `configuracao.json`. Datas e estados: `cronologia.csv`.

## 5. Leitura e reconstrução dos resultados

`medicoes.csv` é a fonte dos contadores por cenário e das medições numéricas entregues. `resultados.csv` é calculado a partir dela, não é uma segunda confirmação independente. Para contadores, somar numeradores e denominadores apenas dentro de um ensaio. Ensaios diferentes podem compartilhar a população. Em resultados.csv, base_de_calculo preserva o total de referência e descricao_base explica o que ele representa. Para média, mediana e diferença entre maior e menor valor, usar a operação indicada. Para percentil_95 de histograma, ordenar valores e localizar o posto teto(0,95 × soma dos pesos). valor_observado reproduz uma medição única. indicador_precalculado transcreve um indicador já calculado: conferir essa transcrição não reproduz seu cálculo original. O LEIA_ME contém o dicionário completo.

Os arquivos não contêm transações bancárias nem uma implementação completa do sistema descrito. São registros primários fictícios do exercício. `entradas.csv` e `observacoes.csv` contêm recortes explicitamente identificados; não pretendem conter todas as entradas da população de um contador. A semente, quando informada, identifica o desenho sintético do cenário, não um executável ausente.

## 6. Limite da conclusão

Conclusão restrita ao catálogo, cinco falhas e três épocas ensaiadas. Nenhum ambiente produtivo é prometido. Pseudocódigo, catálogo sintético, matriz, histogramas e exemplos de trilha permitem auditar a contribuição e os resultados.

## 7. Continuidade e detalhamento técnico

Executar a avaliação de novas regras com precedência e época fixadas, comparando decisões e trilhas ao sequencial-v4 antes de ampliar o catálogo.

As especificações descrevem o recorte sintético e não equivalem a um programa executável de produção.

```text
fixar_epoca_de_cadastro(proposta)
iniciar_regras_com_dependencias_resolvidas_em_paralelo()
ao receber resultado:
  se epoca_diverge: descartar_e_requisitar_epoca_fixada()
  armazenar_resultado_por_id_de_regra()
  percorrer_regras_por_precedencia:
    se regra_prioritaria_pendente: aguardar
    se regra_verdadeira: decidir_e_cancelar_apenas_ramos_sem_influencia()
  gravar_trilha_com_fatos_e_regras_consumidos()
```



`configuracao.json` entrega o catálogo de 148 regras e os seis atrasos por perfil. As dependências são acíclicas. As decisões e histogramas são registros sintéticos, não resultados de execução deste pseudocódigo nesta revisão. Os histogramas entregues são do sequencial-v4 e motor-P3-v3; os contadores de divergência abrangem motor-P1-v1, motor-P2-v2 e motor-P3-v3.
