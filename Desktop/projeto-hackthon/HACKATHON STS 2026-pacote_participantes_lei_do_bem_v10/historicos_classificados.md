# Históricos classificados — V9

Objeto: classificação técnica dos projetos com base somente nas evidências entregues. Horas, despesas e direito fiscal da empresa não integram a decisão.

## PRJ01 — Reprocessamento seguro de mensagens duplicadas

**Não elegível**

O manual antecede o projeto e descreve exatamente o recurso aplicado. O defeito inicial decorre da janela de retenção; corrigir parâmetro e homologar não caracteriza investigação tecnológica.

Limite: Aceite limitado aos conectores e à janela configurada. Não há hipótese de mecanismo novo, apenas adequação da configuração ao contrato.

Divergência do depoimento: 

- **Novidade: NÃO DEMONSTRADA.** O manual fictício BARR-2, seção 4, anterior à configuração, fornece chave composta por operação e versão, retenção de duplicatas e distinção entre reenvio e nova operação. Fonte no projeto: `evidencias/metodo.md#1`.
- **Criatividade técnica: NÃO DEMONSTRADA.** Configurar deduplicação por (operação, versão). Uma retransmissão mantém a chave; uma operação legítima nova recebe outra. Alterar retenção de 30 para 120 segundos, conforme faixa de 10–300 segundos já admitida pelo produto. Nenhum algoritmo do barramento foi modificado. Fonte no projeto: `evidencias/metodo.md#2`.
- **Incerteza tecnológica: NÃO CARACTERIZADA.** Os desvios registrados são resolvidos por configuração, mapeamento ou receita existente, sem hipótese técnica desconhecida. Fonte no projeto: `evidencias/metodo.md#2`.
- **Sistematicidade: DOCUMENTADA COMO ACEITE.** Sessenta roteiros: 40 envios únicos, 12 retransmissões da mesma chave e oito operações novas após interrupção. A rodada deduplicacao-v1 reteve oito duplicatas; deduplicacao-v2 reteve as 12. As oito operações novas foram liberadas em ambas. Fonte no projeto: `evidencias/medicoes.csv`.
- **Transferência/reprodução: DOCUMENTADA PARA A CONFIGURAÇÃO.** Receita, parâmetros, versões e resultados estão localizados; a existência de documentação não transforma rotina em P&D. Fonte no projeto: `evidencias/metodo.md#3`.

## PRJ02 — Detecção de divergências entre razão e extrato

**Elegível**

Método, alternativas, critérios prévios e contagens por perfil tornam verificável a investigação sobre ambiguidade sob desvio de relógios. O ganho não é apenas adoção de um grafo genérico: há regra de restrição e abstenção testada contra esse comparador. A entrevista informa 1.100 pares corretos; evidencias/medicoes.csv e resultados.csv, ensaio PRJ02-S05, registram 1.152/1.200 (96%) em restrito-v4. Prevalecem esses registros identificados por versão e perfil, recalculáveis, sobre o depoimento de memória.

Limite: Conclusão limitada à associação com sequência confiável e aos três deslocamentos ensaiados. Sem inferência sobre transações reais ou ausência de ordem de origem.

Divergência do depoimento: A entrevista informa 1.100 pares corretos; evidencias/medicoes.csv e resultados.csv, ensaio PRJ02-S05, registram 1.152/1.200 (96%) em restrito-v4. Prevalecem esses registros identificados por versão e perfil, recalculáveis, sobre o depoimento de memória.

- **Novidade: DEMONSTRADA NO RECORTE.** Igualdade textual, faixa de valor/tempo e atribuição bipartida de custo mínimo já eram dominadas. A última força pareamento em componentes ambíguos quando dois relógios divergem; não representa causalidade relativa nem abstenção por margem. Fonte no projeto: `evidencias/metodo.md#1`.
- **Criatividade técnica: DEMONSTRADA NO RECORTE.** Grafo de candidatos com arestas apenas para mesmo valor e janela corrigida de 120 s. Estimar deslocamento pela mediana dos pares-âncora inequívocos. Custo de aresta: 0,6×|diferença temporal corrigida|/120 + 0,4×(1-Jaccard dos tokens da descrição em minúsculas). Impor ordem relativa somente dentro de cadeia com sequência confiável, nunca entre contas independentes. Resolver custo mínimo; abster se a margem entre as duas melhores atribuições for menor que 0,15. Bipartido usa o mesmo custo, sem restrição de ordem ou abstenção. Fonte no projeto: `evidencias/metodo.md#2`.
- **Incerteza tecnológica: INVESTIGADA.** Grafo de candidatos com arestas apenas para mesmo valor e janela corrigida de 120 s. Estimar deslocamento pela mediana dos pares-âncora inequívocos. Custo de aresta: 0,6×|diferença temporal corrigida|/120 + 0,4×(1-Jaccard dos tokens da descrição em minúsculas). Impor ordem relativa somente dentro de cadeia com sequência confiável, nunca entre contas independentes. Resolver custo mínimo; abster se a margem entre as duas melhores atribuições for menor que 0,15. Bipartido usa o mesmo custo, sem restrição de ordem ou abstenção. Fonte no projeto: `evidencias/metodo.md#2`.
- **Sistematicidade: DOCUMENTADA.** 1.200 pares, 400 em cada deslocamento de relógio de 0, +90 e -90 segundos; 60 ambíguos por perfil, total 180. Mesma referência lacrada para quatro alternativas. Limites definidos antes da rodada: pelo menos 95% de acerto e no máximo 1% de vínculos falsos. Os 48 pares recusados pelo grafo pertencem ao estrato ambíguo. Fonte no projeto: `evidencias/medicoes.csv`.
- **Transferência/reprodução: DOCUMENTADA NO ESCOPO.** Conclusão limitada à associação com sequência confiável e aos três deslocamentos ensaiados. Sem inferência sobre transações reais ou ausência de ordem de origem. Fonte no projeto: `evidencias/revisao_tecnica.md`.

## PRJ03 — Reexecução controlada após falhas intermitentes

**Elegível**

A incerteza de estabilidade foi investigada com controle comparativo, hipótese operacional e métricas rastreáveis. O escopo e a documentação sustentam a investigação tecnológica registrada.

Limite: Duas falhas e latência nos perfis L1/L2 compõem o escopo de conclusão. Sequências mais longas não foram reivindicadas.

Divergência do depoimento: 

- **Novidade: DEMONSTRADA NO RECORTE.** Lote fixo e controle reativo pela latência eram conhecidos. Após duas falhas sucessivas, atraso da fila e latência do serviço davam sinais incompatíveis: o reativo acelerava ao ver fila envelhecida e recriava sobrecarga. Fonte no projeto: `evidencias/metodo.md#1`.
- **Criatividade técnica: DEMONSTRADA NO RECORTE.** A cada segundo medir p95 da latência em janela de 10 s e derivada do atraso da fila. Limitar lote a [50,1000]. Se p95>400 ms reduzir lote em 25%; se p95≤ 400 ms e atraso cresce, aumentar no máximo 5%; só voltar a acelerar após três janelas estáveis. O acoplamento com histerese, e não um novo nome para retry, foi confrontado com o controlador de latência isolado. Fonte no projeto: `evidencias/metodo.md#2`.
- **Incerteza tecnológica: INVESTIGADA.** A cada segundo medir p95 da latência em janela de 10 s e derivada do atraso da fila. Limitar lote a [50,1000]. Se p95>400 ms reduzir lote em 25%; se p95≤ 400 ms e atraso cresce, aumentar no máximo 5%; só voltar a acelerar após três janelas estáveis. O acoplamento com histerese, e não um novo nome para retry, foi confrontado com o controlador de latência isolado. Fonte no projeto: `evidencias/metodo.md#2`.
- **Sistematicidade: DOCUMENTADA.** Seis cenários: filas de 5, 20 e 50 mil eventos × dois perfis de latência. Falhas nos instantes 30 s e 90 s, indisponibilidade de 10 s cada. Mesmas entradas e calendário nas três estratégias. A redução de 34% refere-se à fila de 50 mil no perfil L2: 1.000 s para 660 s. Fonte no projeto: `evidencias/medicoes.csv`.
- **Transferência/reprodução: DOCUMENTADA NO ESCOPO.** Duas falhas e latência nos perfis L1/L2 compõem o escopo de conclusão. Sequências mais longas não foram reivindicadas. Fonte no projeto: `evidencias/revisao_tecnica.md`.

## PRJ04 — Painel de rastreabilidade da conciliação

**Não elegível**

Há prova suficiente da configuração de funções já contratadas. O resultado é uma melhoria operacional obtida por aplicação conhecida, não P&D.

Limite: Verificação funcional de acesso e visualização. Não se investigou técnica nova de inferência ou reconstrução de trilha.

Divergência do depoimento: 

- **Novidade: NÃO DEMONSTRADA.** O catálogo fictício VIS-3 já fornece junção por correlation_id, mascaramento por perfil e filtros. Os três sistemas de origem emitem esse identificador. Fonte no projeto: `evidencias/metodo.md#1`.
- **Criatividade técnica: NÃO DEMONSTRADA.** Configurar consulta por correlation_id e máscara de campos conforme matriz de acesso. O perfil consulta recebe somente valor mascarado; o perfil conciliação recebe detalhe. Corrigir uma permissão excessiva no mapeamento do perfil. Fonte no projeto: `evidencias/metodo.md#2`.
- **Incerteza tecnológica: NÃO CARACTERIZADA.** Os desvios registrados são resolvidos por configuração, mapeamento ou receita existente, sem hipótese técnica desconhecida. Fonte no projeto: `evidencias/metodo.md#2`.
- **Sistematicidade: DOCUMENTADA COMO ACEITE.** Doze roteiros sobre três sistemas e dois perfis. A primeira matriz permitiu detalhe em um roteiro indevido; a segunda cumpriu os 12 resultados esperados. Fonte no projeto: `evidencias/medicoes.csv`.
- **Transferência/reprodução: DOCUMENTADA PARA A CONFIGURAÇÃO.** Receita, parâmetros, versões e resultados estão localizados; a existência de documentação não transforma rotina em P&D. Fonte no projeto: `evidencias/metodo.md#3`.

## PRJ05 — Aplicativo de proposta com trabalho offline

**Com ressalvas**

O método proposto e a comparação documentam P&D no recorte ensaiado. A lacuna é técnica e específica: persistência da revogação sob corte de energia. A cobertura de segurança permanece limitada aos tipos de interrupção ensaiados. A entrevista afirma ausência de leitura indevida no banco local; evidencias/medicoes.csv e resultados.csv registram 2/8 em banco-v1 (PRJ05-S02) e 0/8 em epoca-v2 (PRJ05-S04). Prevalece a distinção documental entre as versões; a fala não sustenta ausência de falhas no comparador.

Limite: O protocolo inclui uma hipótese sobre interrupção abrupta entre gravação do marcador e eliminação da chave. Corte de energia nessa janela não foi ensaiado. A evidência sustenta a investigação e a solução para quedas de rede, mas não a alegação de revogação em qualquer encerramento.

Divergência do depoimento: A entrevista afirma ausência de leitura indevida no banco local; evidencias/medicoes.csv e resultados.csv registram 2/8 em banco-v1 (PRJ05-S02) e 0/8 em epoca-v2 (PRJ05-S04). Prevalece a distinção documental entre as versões; a fala não sustenta ausência de falhas no comparador.

- **Novidade: DEMONSTRADA NO RECORTE.** Banco local cifrado e chave por sessão eram conhecidos. O comparador BASE-OFF-1 usa uma chave persistente por perfil; a alternativa chave efêmera por formulário já conhecida perde recuperação após suspensão. O problema investigado é recuperar blocos após queda de rede e suspender acesso após troca de usuário, com transições concorrentes do processo. Fonte no projeto: `evidencias/metodo.md#1`.
- **Criatividade técnica: DEMONSTRADA NO RECORTE.** Protótipo mantém blocos cifrados associados a época de sessão. Antes de trocar usuário, grava marcador de revogação e incrementa época; na retomada, aceita bloco apenas se época do cabeçalho coincide com a sessão e o marcador não o revoga. A hipótese combina recuperação de gravação parcial e invalidação lógica antes da remoção física da chave. Não envolve inventar cifra ou apresentar dados reais. Fonte no projeto: `evidencias/metodo.md#2`.
- **Incerteza tecnológica: INVESTIGADA.** Protótipo mantém blocos cifrados associados a época de sessão. Antes de trocar usuário, grava marcador de revogação e incrementa época; na retomada, aceita bloco apenas se época do cabeçalho coincide com a sessão e o marcador não o revoga. A hipótese combina recuperação de gravação parcial e invalidação lógica antes da remoção física da chave. Não envolve inventar cifra ou apresentar dados reais. Fonte no projeto: `evidencias/metodo.md#2`.
- **Sistematicidade: DOCUMENTADA.** 24 roteiros em três pontos de queda de rede e duas trocas de sessão. Banco local recuperou 24, mas deixou dois roteiros de troca acessíveis ao usuário seguinte. Blocos por época recuperaram 24 e bloquearam leitura indevida nas oito trocas. Há oito testes de retomada por alternativa efêmera, dos quais seis perderam o formulário. Fonte no projeto: `evidencias/medicoes.csv`.
- **Transferência/reprodução: DOCUMENTADA COM LIMITE.** O protocolo inclui uma hipótese sobre interrupção abrupta entre gravação do marcador e eliminação da chave. Corte de energia nessa janela não foi ensaiado. A evidência sustenta a investigação e a solução para quedas de rede, mas não a alegação de revogação em qualquer encerramento. Fonte no projeto: `evidencias/revisao_tecnica.md`.

## PRJ06 — Sincronização de propostas após reconexão

**Com ressalvas**

Há método e evidência de comparação suficiente para identificar investigação. A ressalva incide sobre exclusão concorrente referenciada, não sobre ausência abstrata de reprodução por outra equipe.

Limite: As listas ensaiadas contêm inserção e edição, mas não exclusão concorrente de item já referenciado por anexo. A hipótese de preservação dessa dependência permanece em aberto e integra a pretensão original.

Divergência do depoimento: 

- **Novidade: DEMONSTRADA NO RECORTE.** Última gravação vence ignora intenção; mesclagem de três vias preserva campos independentes, mas não a relação entre valor e anexo quando edições offline mudam ambos. Esses dois comparadores estão especificados e medidos. Fonte no projeto: `evidencias/metodo.md#1`.
- **Criatividade técnica: DEMONSTRADA NO RECORTE.** Representar edição como (campo, versão-base, intenção, dependência). Mesclar operações de campos independentes; para valor e anexo, aceitar automaticamente somente se a versão-base do documento confirma o mesmo valor. Em conflito dependente, produzir duas alternativas preservadas para escolha humana. A incerteza era obter automação sem apagar a dependência semântica. Fonte no projeto: `evidencias/metodo.md#2`.
- **Incerteza tecnológica: INVESTIGADA.** Representar edição como (campo, versão-base, intenção, dependência). Mesclar operações de campos independentes; para valor e anexo, aceitar automaticamente somente se a versão-base do documento confirma o mesmo valor. Em conflito dependente, produzir duas alternativas preservadas para escolha humana. A incerteza era obter automação sem apagar a dependência semântica. Fonte no projeto: `evidencias/metodo.md#2`.
- **Sistematicidade: DOCUMENTADA.** 90 conflitos: 30 campos simples, 30 anexos e 30 listas. Mesmas versões iniciais e edições em todas as estratégias. O conjunto contém 16 conflitos dependentes que exigem decisão humana. Última gravação perde conteúdo em 11; mesclagem proposta resolve 74 e encaminha 16 sem perda silenciosa. Fonte no projeto: `evidencias/medicoes.csv`.
- **Transferência/reprodução: DOCUMENTADA COM LIMITE.** As listas ensaiadas contêm inserção e edição, mas não exclusão concorrente de item já referenciado por anexo. A hipótese de preservação dessa dependência permanece em aberto e integra a pretensão original. Fonte no projeto: `evidencias/revisao_tecnica.md`.

## PRJ07 — Autenticação em aparelhos simples

**Com ressalvas**

Comparadores, transições, orçamento de memória e registros sustentam o núcleo de investigação. A ressalva é a falta do ataque de adulteração local previsto, sem converter vinte replays em prova de segurança geral. A entrevista menciona um replay aceito na versão em etapas; evidencias/medicoes.csv e resultados.csv, PRJ07-S05, registram 0/20 em etapas-v3. Prevalece o contador identificado dessa versão, sem ampliar o resultado para resistência geral à adulteração.

Limite: Não há teste de substituição do conteúdo entre captura e assinatura em processo comprometido. A conclusão anti-replay tem amostra delimitada; a pretensão de resistência a adulteração local ainda não foi validada.

Divergência do depoimento: A entrevista menciona um replay aceito na versão em etapas; evidencias/medicoes.csv e resultados.csv, PRJ07-S05, registram 0/20 em etapas-v3. Prevalece o contador identificado dessa versão, sem ampliar o resultado para resistência geral à adulteração.

- **Novidade: DEMONSTRADA NO RECORTE.** Biblioteca completa e versão leve de quadro único já estavam disponíveis. A completa usa 82 MB no pico; a leve cabe no limite de 32 MB, mas não vincula retomada ao desafio anterior quando a rede cai. Fonte no projeto: `evidencias/metodo.md#1`.
- **Criatividade técnica: DEMONSTRADA NO RECORTE.** Dividir captura, assinatura do transcript e envio. Cada etapa consome a anterior por nonce de uso único; assinatura cobre hash do transcript, época de sessão e contador da etapa. Retomar somente após confirmar consumo do último nonce; liberar buffers de imagem antes da assinatura. O problema era preservar vínculo entre etapas sob memória limitada e interrupção, não criar um algoritmo criptográfico. Fonte no projeto: `evidencias/metodo.md#2`.
- **Incerteza tecnológica: INVESTIGADA.** Dividir captura, assinatura do transcript e envio. Cada etapa consome a anterior por nonce de uso único; assinatura cobre hash do transcript, época de sessão e contador da etapa. Retomar somente após confirmar consumo do último nonce; liberar buffers de imagem antes da assinatura. O problema era preservar vínculo entre etapas sob memória limitada e interrupção, não criar um algoritmo criptográfico. Fonte no projeto: `evidencias/metodo.md#2`.
- **Sistematicidade: DOCUMENTADA.** 300 sessões legítimas: três perfis de aparelho × quatro níveis de sinal × 25 sessões. Vinte replays em conjunto separado. Completa conclui 183/300, leve 255/300 e etapas 276/300; a leve aceita três replays e etapas nenhum dos 20. Fonte no projeto: `evidencias/medicoes.csv`.
- **Transferência/reprodução: DOCUMENTADA COM LIMITE.** Não há teste de substituição do conteúdo entre captura e assinatura em processo comprometido. A conclusão anti-replay tem amostra delimitada; a pretensão de resistência a adulteração local ainda não foi validada. Fonte no projeto: `evidencias/revisao_tecnica.md`.

## PRJ08 — Monitoramento de canais em agências remotas

**Evidência insuficiente**

A entrega permite verificar preparação e coleta parcial. Não permite distinguir se houve investigação técnica ou apenas integração de sondas, nem auditar a alegação de separação de causas.

Limite: Faltam versão do classificador, causas de referência e decisões por evento. Os registros sintéticos documentam a coleta, mas não o resultado alegado de classificação.

Divergência do depoimento: 

- **Novidade: INDETERMINADA.** Há um extrato de catálogo de sondas TCP/HTTP e uma topologia de três pontos. A equipe propôs distinguir enlace, aplicação e equipamento, mas não preservou uma regra de decisão executada. Fonte no projeto: `evidencias/metodo.md#1`.
- **Criatividade técnica: INDETERMINADA.** Plano propõe comparar latência local, disponibilidade do endpoint e heartbeat. Documento de desenho contém tabela de combinações possíveis, sem limiares aprovados nem associação das medições a causas injetadas. Fonte no projeto: `evidencias/metodo.md#2`.
- **Incerteza tecnológica: ALEGADA, NÃO VERIFICÁVEL.** Faltam versão do classificador, causas de referência e decisões por evento. Os registros sintéticos documentam a coleta, mas não o resultado alegado de classificação. Fonte no projeto: `evidencias/revisao_tecnica.md`.
- **Sistematicidade: PARCIAL.** Seis medições de disponibilidade foram recuperadas com timestamps e origem. Nenhuma tem causa controlada ou saída do classificador. Um memorando de demonstração afirma três causas separadas, sem vincular o número a esses seis registros. Fonte no projeto: `evidencias/medicoes.csv`.
- **Transferência/reprodução: INSUFICIENTE PARA O NÚCLEO ALEGADO.** Faltam versão do classificador, causas de referência e decisões por evento. Os registros sintéticos documentam a coleta, mas não o resultado alegado de classificação. Fonte no projeto: `evidencias/revisao_tecnica.md`.

## PRJ09 — Gateway adaptativo para versões de API

**Não elegível**

Os contratos e a configuração evidenciam aplicação de mecanismo existente. Testes de regressão e correção de permissão não introduzem P&D.

Limite: O termo adaptativo no título comercial se refere à escolha de rota por versão. Não há inferência ou técnica nova de adaptação.

Divergência do depoimento: 

- **Novidade: NÃO DEMONSTRADA.** O manual fictício GATE-4 define roteamento por cabeçalho api_version, transformação de campos e validação por contratos. A receita de coexistência é anterior à equipe. Fonte no projeto: `evidencias/metodo.md#1`.
- **Criatividade técnica: NÃO DEMONSTRADA.** Cadastrar rotas v1/v2 e mapear campo total_centavos para total_decimal dividindo por 100; preservar campos obrigatórios. Ajustar escopo de permissão do conector, sem modificar motor de roteamento. Fonte no projeto: `evidencias/metodo.md#2`.
- **Incerteza tecnológica: NÃO CARACTERIZADA.** Os desvios registrados são resolvidos por configuração, mapeamento ou receita existente, sem hipótese técnica desconhecida. Fonte no projeto: `evidencias/metodo.md#2`.
- **Sistematicidade: DOCUMENTADA COMO ACEITE.** Vinte contratos por versão, 40 ao todo. Uma permissão incorreta na primeira rodada bloqueou dois contratos; todos passaram após correção. Fonte no projeto: `evidencias/medicoes.csv`.
- **Transferência/reprodução: DOCUMENTADA PARA A CONFIGURAÇÃO.** Receita, parâmetros, versões e resultados estão localizados; a existência de documentação não transforma rotina em P&D. Fonte no projeto: `evidencias/metodo.md#3`.

## PRJ10 — Gestão de consentimentos em múltiplos canais

**Evidência insuficiente**

Há documentação de intenção e eventos parciais, mas não o elo entre transições, versão e resultado. Não é possível concluir P&D nem confirmar que todo o escopo era rotina.

Limite: Faltam versão executada, ordem causal confiável e saídas de estado. O diagrama por si só não prova execução do mecanismo proposto.

Divergência do depoimento: 

- **Novidade: INDETERMINADA.** O fluxo de consentimento prevê iniciar no aplicativo e cancelar no portal. Existe diagrama com autorização, revogação e expiração, mas nenhuma tabela de precedência de eventos concorrentes aprovada. Fonte no projeto: `evidencias/metodo.md#1`.
- **Criatividade técnica: INDETERMINADA.** Minuta da máquina de estados lista iniciar→pendente→autorizado e revogar→revogado. Não define empate entre confirmação e revogação, relógio de referência ou política de idempotência. Fonte no projeto: `evidencias/metodo.md#2`.
- **Incerteza tecnológica: ALEGADA, NÃO VERIFICÁVEL.** Faltam versão executada, ordem causal confiável e saídas de estado. O diagrama por si só não prova execução do mecanismo proposto. Fonte no projeto: `evidencias/revisao_tecnica.md`.
- **Sistematicidade: PARCIAL.** Foram entregues oito eventos sintéticos de navegação e o diagrama. Quatro eventos têm um identificador de sessão e quatro não; nenhum contém a decisão de estado retornada pelo serviço. O memorando afirma redução de divergências sem preservar o antes/depois. Fonte no projeto: `evidencias/medicoes.csv`.
- **Transferência/reprodução: INSUFICIENTE PARA O NÚCLEO ALEGADO.** Faltam versão executada, ordem causal confiável e saídas de estado. O diagrama por si só não prova execução do mecanismo proposto. Fonte no projeto: `evidencias/revisao_tecnica.md`.

## PRJ11 — Normalização de dados de contas

**Não elegível**

Transformações determinísticas já definidas, cadastro e aceite estão demonstrados. A automação de um trabalho útil não basta para caracterizar P&D.

Limite: O escopo é conformidade com dicionário aprovado. Nenhum novo método de normalização foi proposto ou testado.

Divergência do depoimento: 

- **Novidade: NÃO DEMONSTRADA.** O dicionário fictício DIC-11/v3 antecede o projeto: tipo CC→corrente, PP→poupança; data ISO; valor ausente permanece nulo; origem desconhecida é rejeitada. Fonte no projeto: `evidencias/metodo.md#1`.
- **Criatividade técnica: NÃO DEMONSTRADA.** Aplicar mapeamento por origem e validar tipo, domínio e ausência. Não inferir categorias nem estimar valores. Corrigir três cadastros de origem e repetir a mesma carga. Fonte no projeto: `evidencias/metodo.md#2`.
- **Incerteza tecnológica: NÃO CARACTERIZADA.** Os desvios registrados são resolvidos por configuração, mapeamento ou receita existente, sem hipótese técnica desconhecida. Fonte no projeto: `evidencias/metodo.md#2`.
- **Sistematicidade: DOCUMENTADA COMO ACEITE.** Cem registros sintéticos de duas origens; três rejeições por código fora do dicionário na primeira passagem. Após correção cadastral, 100 conformes. Fonte no projeto: `evidencias/medicoes.csv`.
- **Transferência/reprodução: DOCUMENTADA PARA A CONFIGURAÇÃO.** Receita, parâmetros, versões e resultados estão localizados; a existência de documentação não transforma rotina em P&D. Fonte no projeto: `evidencias/metodo.md#3`.

## PRJ12 — Cofre de chaves para integrações externas

**Não elegível**

Evidências suficientes mostram integração e parametrização da funcionalidade contratada. O problema encontrado é resolvido diretamente pela instrução do manual.

Limite: Não há criação de primitiva criptográfica, protocolo de rotação ou investigação de problema sem solução conhecida.

Divergência do depoimento: 

- **Novidade: NÃO DEMONSTRADA.** O cofre fictício COF-2 permite referência simbólica à versão, chave ativa e anterior, e coexistência durante rotação. O manual recomenda janela maior que o atraso máximo dos conectores. Fonte no projeto: `evidencias/metodo.md#1`.
- **Criatividade técnica: NÃO DEMONSTRADA.** Configurar referências KEY-SINT-V1/V2, sem conteúdo de chave. Ampliar janela de coexistência de 30 para 120 segundos diante de atraso de 70 segundos no conector. Aplicar receita do fornecedor. Fonte no projeto: `evidencias/metodo.md#2`.
- **Incerteza tecnológica: NÃO CARACTERIZADA.** Os desvios registrados são resolvidos por configuração, mapeamento ou receita existente, sem hipótese técnica desconhecida. Fonte no projeto: `evidencias/metodo.md#2`.
- **Sistematicidade: DOCUMENTADA COMO ACEITE.** Dezoito roteiros de cifração, decifração e rotação em três conectores. Duas falhas com janela curta e 18 aprovações depois do ajuste. Fonte no projeto: `evidencias/medicoes.csv`.
- **Transferência/reprodução: DOCUMENTADA PARA A CONFIGURAÇÃO.** Receita, parâmetros, versões e resultados estão localizados; a existência de documentação não transforma rotina em P&D. Fonte no projeto: `evidencias/metodo.md#3`.

## PRJ13 — Modelo de comportamento transacional

**Elegível**

A quarentena e a abertura condicionada de faixa são explicitadas e confrontadas com métodos conhecidos. Contagens de legítimas e fraudes separadas demonstram critério e resultado, sem confundir percentual de falso alerta com prevalência de fraude. A entrevista cita 2,8% de falsos alertas; evidencias/medicoes.csv e resultados.csv, PRJ13-S04, mostram 1.116/36.000 (3,1%) em condicionado-v4. Prevalece a agregação documental das legítimas nos quatro perfis, com denominador explícito.

Limite: A documentação permite reconstruir as contagens nos quatro deslocamentos sintéticos. Não se reivindica validade para distribuições externas.

Divergência do depoimento: A entrevista cita 2,8% de falsos alertas; evidencias/medicoes.csv e resultados.csv, PRJ13-S04, mostram 1.116/36.000 (3,1%) em condicionado-v4. Prevalece a agregação documental das legítimas nos quatro perfis, com denominador explícito.

- **Novidade: DEMONSTRADA NO RECORTE.** Limite fixo, perfil mensal e atualização exponencial contínua são conhecidos. Na sequência estudada, a atualização contínua incorpora eventos ainda não revisados e desloca o perfil em direção a ataques persistentes. Fonte no projeto: `evidencias/metodo.md#1`.
- **Criatividade técnica: DEMONSTRADA NO RECORTE.** Manter centro e dispersão robustos por segmento. Eventos alertados entram em quarentena; só atualizar o perfil após confirmação de legitimidade. Abrir nova faixa sazonal quando a mudança persiste por três janelas sem aumento de rótulos fraudulentos. Usar faixa de confiança de 2,5 desvios nos segmentos frequentes e 3,0 nos dois segmentos escassos, definidos na preparação. A hipótese é distinguir mudança legítima e contaminação sem perder sensibilidade. Fonte no projeto: `evidencias/metodo.md#2`.
- **Incerteza tecnológica: INVESTIGADA.** Manter centro e dispersão robustos por segmento. Eventos alertados entram em quarentena; só atualizar o perfil após confirmação de legitimidade. Abrir nova faixa sazonal quando a mudança persiste por três janelas sem aumento de rótulos fraudulentos. Usar faixa de confiança de 2,5 desvios nos segmentos frequentes e 3,0 nos dois segmentos escassos, definidos na preparação. A hipótese é distinguir mudança legítima e contaminação sem perder sensibilidade. Fonte no projeto: `evidencias/metodo.md#2`.
- **Sistematicidade: DOCUMENTADA.** 40 mil transações: 36 mil legítimas e quatro mil fraudes, distribuídas igualmente em quatro deslocamentos sazonais. Comparar limite fixo, perfil mensal, atualização contínua e atualização condicionada. Todos usam mesma ordem, rótulos e corte temporal; rótulo só é disponibilizado ao atualizador após atraso de 20 eventos. Critérios prévios: falso alerta ≤ 4% nas legítimas e sensibilidade>90% em cada deslocamento. Fonte no projeto: `evidencias/medicoes.csv`.
- **Transferência/reprodução: DOCUMENTADA NO ESCOPO.** A documentação permite reconstruir as contagens nos quatro deslocamentos sintéticos. Não se reivindica validade para distribuições externas. Fonte no projeto: `evidencias/revisao_tecnica.md`.

## PRJ14 — Detecção de novos padrões de golpe

**Elegível**

Há diferença técnica definida no ciclo de vida de arestas, hipóteses, comparador temporal e resultados por campanha. Método e carga de revisão sustentam o enquadramento no recorte comprovado.

Limite: Capacidade de revisão prevista é 25 alertas adicionais por lote. O experimento cobre 14 campanhas, não todo tipo de golpe.

Divergência do depoimento: 

- **Novidade: DEMONSTRADA NO RECORTE.** Pontuação individual e grafo temporal com expiração fixa foram especificados como comparadores. O primeiro ignora vínculos; o segundo apaga a cadeia quando contas alternam destinos perto do fim da janela. Fonte no projeto: `evidencias/metodo.md#1`.
- **Criatividade técnica: DEMONSTRADA NO RECORTE.** Representar aresta por evento e predecessores causais. Manter aresta enquanto houver descendente confirmado dentro do horizonte de 15 minutos, com teto absoluto de 45 minutos. Fechar componente ao perder continuidade causal. Pontuar recorrência de caminho e redistribuição de destinos, sem usar nome da conta como rótulo. Testar se esse ciclo de vida recupera campanhas perdidas pela expiração fixa, sem ultrapassar 25 alertas adicionais por lote. Fonte no projeto: `evidencias/metodo.md#2`.
- **Incerteza tecnológica: INVESTIGADA.** Representar aresta por evento e predecessores causais. Manter aresta enquanto houver descendente confirmado dentro do horizonte de 15 minutos, com teto absoluto de 45 minutos. Fechar componente ao perder continuidade causal. Pontuar recorrência de caminho e redistribuição de destinos, sem usar nome da conta como rótulo. Testar se esse ciclo de vida recupera campanhas perdidas pela expiração fixa, sem ultrapassar 25 alertas adicionais por lote. Fonte no projeto: `evidencias/metodo.md#2`.
- **Sistematicidade: DOCUMENTADA.** 14 campanhas em 120 mil eventos normais. Três alternativas na mesma sequência; verdade de referência antes da comparação. Pontuação individual detecta seis campanhas, temporal nove e causal treze. Os 22 alertas adicionais do causal frente ao individual incluem sete verdadeiros de campanhas distintas recuperadas e 15 falsos. Fonte no projeto: `evidencias/medicoes.csv`.
- **Transferência/reprodução: DOCUMENTADA NO ESCOPO.** Capacidade de revisão prevista é 25 alertas adicionais por lote. O experimento cobre 14 campanhas, não todo tipo de golpe. Fonte no projeto: `evidencias/revisao_tecnica.md`.

## PRJ15 — Explicação de alertas para revisão humana

**Elegível**

A hipótese sobre snapshot e dependência está operacionalizada e o defeito é verificável em dois casos. Os registros de leitura, versões e replay sustentam sistematicidade e transferência. A entrevista atribui 78/80 rastros fiéis ao encerramento; evidencias/medicoes.csv e resultados.csv registram 78/80 em rastro-v1 (PRJ15-S01) e 80/80 em rastro-v2 (PRJ15-S02). Prevalece o resultado documental da versão final rastro-v2; 78/80 pertence à versão anterior.

Limite: O resultado mede fidelidade ao cálculo e tempo de revisão, não qualidade do juízo de crédito ou consenso entre analistas reais.

Divergência do depoimento: A entrevista atribui 78/80 rastros fiéis ao encerramento; evidencias/medicoes.csv e resultados.csv registram 78/80 em rastro-v1 (PRJ15-S01) e 80/80 em rastro-v2 (PRJ15-S02). Prevalece o resultado documental da versão final rastro-v2; 78/80 pertence à versão anterior.

- **Novidade: DEMONSTRADA NO RECORTE.** Texto padrão e lista dos maiores fatores explicavam valores presentes ao final da análise, que podiam diferir dos usados na decisão após enriquecimento tardio. Fonte no projeto: `evidencias/metodo.md#1`.
- **Criatividade técnica: DEMONSTRADA NO RECORTE.** Capturar snapshot de versão e fatos efetivamente consumidos em cada regra. Construir explicação apenas das dependências que contribuíram ao caminho escolhido, com referência ao snapshot, nunca ao cadastro mais recente. Ao reavaliar, abrir nova trilha com parent_id da anterior. Confrontar cada explicação com replay da mesma versão do motor. Fonte no projeto: `evidencias/metodo.md#2`.
- **Incerteza tecnológica: INVESTIGADA.** Capturar snapshot de versão e fatos efetivamente consumidos em cada regra. Construir explicação apenas das dependências que contribuíram ao caminho escolhido, com referência ao snapshot, nunca ao cadastro mais recente. Ao reavaliar, abrir nova trilha com parent_id da anterior. Confrontar cada explicação com replay da mesma versão do motor. Fonte no projeto: `evidencias/metodo.md#2`.
- **Sistematicidade: DOCUMENTADA.** 80 alertas; versões rastro-v1 e rastro-v2 confrontadas com replay. Dois enriquecimentos tardios divergiram em rastro-v1; em rastro-v2 os 80 correspondem. Vinte analistas sintéticos A01–A20: cada um lê quatro alertas por método; permutação de ordem contrabalançada consta na configuração e nos registros de leitura. Medianas de 80 tempos por método: 6,4 e 4,1 minutos. Fonte no projeto: `evidencias/medicoes.csv`.
- **Transferência/reprodução: DOCUMENTADA NO ESCOPO.** O resultado mede fidelidade ao cálculo e tempo de revisão, não qualidade do juízo de crédito ou consenso entre analistas reais. Fonte no projeto: `evidencias/revisao_tecnica.md`.

## PRJ16 — Simulador de ataques ao fluxo de pagamento

**Elegível**

A técnica de liberação causal e desempate é documentada nas próprias evidências. Comparadores e repetições distinguem investigação de mera execução de testes.

Limite: O pacote contém grafo de exemplo, regra de ordenação, configuração e registros por cenário. Não inclui ambiente de integração; a transferência é da especificação e dos resultados sintéticos.

Divergência do depoimento: 

- **Novidade: DEMONSTRADA NO RECORTE.** Roteiro manual e replay por tempo de captura reproduzem requisições, mas podem inverter precedência quando a troca de dispositivo altera o atraso de processamento. Fonte no projeto: `evidencias/metodo.md#1`.
- **Criatividade técnica: DEMONSTRADA NO RECORTE.** Grafo acíclico com eventos e dependências; fila de prontos ordenada por (tempo lógico, sequência da origem, ID do evento). Um relógio virtual de injeção controla atrasos, sem substituir o relógio interno do serviço. Só liberar evento depois dos predecessores confirmados. Empates são resolvidos por sequência/ID, correção documentada em causal-v3. Fonte no projeto: `evidencias/metodo.md#2`.
- **Incerteza tecnológica: INVESTIGADA.** Grafo acíclico com eventos e dependências; fila de prontos ordenada por (tempo lógico, sequência da origem, ID do evento). Um relógio virtual de injeção controla atrasos, sem substituir o relógio interno do serviço. Só liberar evento depois dos predecessores confirmados. Empates são resolvidos por sequência/ID, correção documentada em causal-v3. Fonte no projeto: `evidencias/metodo.md#2`.
- **Sistematicidade: DOCUMENTADA.** 32 ataques com cinco repetições cada, total 160. Manual, replay e gerador causal com mesma matriz de entrada. Critério: ordem parcial válida e estado terminal equivalente. Duas falhas intermitentes são identificadas por cenário e reproduzidas nas cinco repetições; não significam duas falhas em cada execução. Fonte no projeto: `evidencias/medicoes.csv`.
- **Transferência/reprodução: DOCUMENTADA NO ESCOPO.** O pacote contém grafo de exemplo, regra de ordenação, configuração e registros por cenário. Não inclui ambiente de integração; a transferência é da especificação e dos resultados sintéticos. Fonte no projeto: `evidencias/revisao_tecnica.md`.

## PRJ17 — Simulação de políticas de crédito

**Evidência insuficiente**

A documentação é insuficiente para distinguir comparação rotineira de políticas de uma investigação nova. A lacuna afeta o próprio objeto e resultado, não apenas a amplitude de validação.

Limite: Sem regras e saídas não se avalia diferença técnica ou o número alegado. Os arquivos de entrada existentes não completam a cadeia.

Divergência do depoimento: 

- **Novidade: INDETERMINADA.** O plano contrapõe política vigente e política candidata, mas as regras são identificadas apenas como antiga/nova. Não há tabelas de limiares nem versão assinada para nenhuma. Fonte no projeto: `evidencias/metodo.md#1`.
- **Criatividade técnica: INDETERMINADA.** Diagrama propõe duplicar uma proposta, executar duas políticas sem emitir contrato e comparar aprovação. Não define se o motor foi alterado ou se eram parâmetros do simulador comercial. Fonte no projeto: `evidencias/metodo.md#2`.
- **Incerteza tecnológica: ALEGADA, NÃO VERIFICÁVEL.** Sem regras e saídas não se avalia diferença técnica ou o número alegado. Os arquivos de entrada existentes não completam a cadeia. Fonte no projeto: `evidencias/revisao_tecnica.md`.
- **Sistematicidade: PARCIAL.** Há seis propostas sintéticas com renda e comprometimento e um memorando de demonstração que afirma impacto diferente. Não há decisões pareadas nem versão das políticas; as seis propostas não têm vínculo com a demonstração. Fonte no projeto: `evidencias/medicoes.csv`.
- **Transferência/reprodução: INSUFICIENTE PARA O NÚCLEO ALEGADO.** Sem regras e saídas não se avalia diferença técnica ou o número alegado. Os arquivos de entrada existentes não completam a cadeia. Fonte no projeto: `evidencias/revisao_tecnica.md`.

## PRJ18 — Motor de regras auditável

**Com ressalvas**

Há P&D demonstrado pela hipótese de registro por mudança e comparação. Com ressalvas porque uma limitação técnica conhecida impede estender a afirmação de auditabilidade a todas as decisões. Falhar no objetivo não elimina o caráter de pesquisa. A entrevista afirma que todas as decisões foram explicadas pela trilha seletiva; evidencias/medicoes.csv e resultados.csv, PRJ18-S03, registram 96% em seletivo-v3. Prevalece o registro dessa versão; a cobertura incompleta é coerente com a pendência de callback tardio.

Limite: O tratamento de callback tardio segue aberto; o protótipo não satisfaz a exigência de explicação integral. A cronologia identifica as versões e o critério da rodada.

Divergência do depoimento: A entrevista afirma que todas as decisões foram explicadas pela trilha seletiva; evidencias/medicoes.csv e resultados.csv, PRJ18-S03, registram 96% em seletivo-v3. Prevalece o registro dessa versão; a cobertura incompleta é coerente com a pendência de callback tardio.

- **Novidade: DEMONSTRADA NO RECORTE.** Log integral preserva fatos mas amplia latência; amostragem periódica perde mudanças de estado entre pontos. Ambos foram comparados na mesma carga, com o mesmo motor e regra de explicação. Fonte no projeto: `evidencias/metodo.md#1`.
- **Criatividade técnica: DEMONSTRADA NO RECORTE.** Emitir registro quando muda o estado lógico de regra, incluindo snapshot de fatos e ponteiro para predecessor; coalescer leituras sem mudança. A hipótese é conservar dependências decisórias com menos escrita. A versão seletiva ainda perde o predecessor quando um callback tardio chega após a coalescência. Fonte no projeto: `evidencias/metodo.md#2`.
- **Incerteza tecnológica: INVESTIGADA.** Emitir registro quando muda o estado lógico de regra, incluindo snapshot de fatos e ponteiro para predecessor; coalescer leituras sem mudança. A hipótese é conservar dependências decisórias com menos escrita. A versão seletiva ainda perde o predecessor quando um callback tardio chega após a coalescência. Fonte no projeto: `evidencias/metodo.md#2`.
- **Sistematicidade: DOCUMENTADA.** 50 mil decisões, quatro perfis. Log integral explica 100% com acréscimo de 24% na latência média; amostragem explica 82% com 3%; seletivo explica 96% com 7%. Critério prévio pretendia explicação de 100% e aumento de latência ≤ 10%. Os 2.000 casos sem trilha completa permanecem identificados por perfil. Fonte no projeto: `evidencias/medicoes.csv`.
- **Transferência/reprodução: DOCUMENTADA COM LIMITE.** O tratamento de callback tardio segue aberto; o protótipo não satisfaz a exigência de explicação integral. A cronologia identifica as versões e o critério da rodada. Fonte no projeto: `evidencias/revisao_tecnica.md`.

## PRJ19 — Leitura de documentos de produtores

**Não elegível**

Os registros sustentam implementação de recurso comercial já disponível. O uso de OCR e a complexidade dos documentos não mudam a natureza rotineira do projeto.

Limite: As verificações são de aceite do produto e de sua fila de exceções. Não demonstram desenvolvimento de outro método de leitura.

Divergência do depoimento: 

- **Novidade: NÃO DEMONSTRADA.** O produto fictício OCR-5 já oferece correção de inclinação, extração de campos e fila humana abaixo do limiar de confiança. O manual define uso desse limiar. Fonte no projeto: `evidencias/metodo.md#1`.
- **Criatividade técnica: NÃO DEMONSTRADA.** Aplicar OCR contratado, mapear campos previstos e fixar limiar 0,85. Imagens cortadas ou abaixo desse valor vão à revisão humana. Não treinar modelo nem alterar pré-processador. Fonte no projeto: `evidencias/metodo.md#2`.
- **Incerteza tecnológica: NÃO CARACTERIZADA.** Os desvios registrados são resolvidos por configuração, mapeamento ou receita existente, sem hipótese técnica desconhecida. Fonte no projeto: `evidencias/metodo.md#2`.
- **Sistematicidade: DOCUMENTADA COMO ACEITE.** Trinta imagens sintéticas: 20 legíveis, cinco com corte e cinco com sombra forte. Extrair campos das 20 legíveis e enviar as dez restantes à fila. Fonte no projeto: `evidencias/medicoes.csv`.
- **Transferência/reprodução: DOCUMENTADA PARA A CONFIGURAÇÃO.** Receita, parâmetros, versões e resultados estão localizados; a existência de documentação não transforma rotina em P&D. Fonte no projeto: `evidencias/metodo.md#3`.

## PRJ20 — Coexistência entre motor antigo e novo

**Não elegível**

O escopo e as evidências demonstram uso integral de modo sombra conhecido, sem contribuição investigativa adicional. Diferenças de decisão são diferenças de política, não falha científica a resolver.

Limite: Resultados não demonstram avanço tecnológico. Executar novamente pode ampliar confiança operacional, mas não transforma a configuração documentada em pesquisa.

Divergência do depoimento: 

- **Novidade: NÃO DEMONSTRADA.** A plataforma fictícia SIM-4 oferece modo sombra, duplicação da solicitação, supressão de efeitos e comparação de saídas. O manual anterior prevê especificamente que apenas o motor principal emita contratos. Fonte no projeto: `evidencias/metodo.md#1`.
- **Criatividade técnica: NÃO DEMONSTRADA.** Ativar shadow=true, write_side_effects=false e comparar campos decisao e motivo. Selecionar divergências para revisão de negócio. As regras e o mecanismo de isolamento foram fornecidos pela plataforma; não houve alteração do comparador ou hipótese sobre mecanismo novo. Fonte no projeto: `evidencias/metodo.md#2`.
- **Incerteza tecnológica: NÃO CARACTERIZADA.** Os desvios registrados são resolvidos por configuração, mapeamento ou receita existente, sem hipótese técnica desconhecida. Fonte no projeto: `evidencias/metodo.md#2`.
- **Sistematicidade: DOCUMENTADA COMO ACEITE.** 18 mil propostas idênticas enviadas aos dois motores. 430 divergências correspondem às diferenças de política cadastradas; zero contratos emitidos pelo motor sombra. O roteiro verifica configuração e equivalência de entradas. Fonte no projeto: `evidencias/medicoes.csv`.
- **Transferência/reprodução: DOCUMENTADA PARA A CONFIGURAÇÃO.** Receita, parâmetros, versões e resultados estão localizados; a existência de documentação não transforma rotina em P&D. Fonte no projeto: `evidencias/metodo.md#3`.

