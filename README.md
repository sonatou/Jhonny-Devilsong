# Steering Behaviors Allan Oliveira
**Exercício Grau A | Inteligência Artificial para Jogos | Unisinos**

---

## O que é isso?

Este projeto implementa cinco Steering Behaviors clássicos de Craig Reynolds em Unity.
A ideia é simples: você controla um personagem que caminha por salas, e em cada sala
um NPC demonstra um comportamento diferente de movimentação autônoma.


---

## Os Behaviours

### Seek
O NPC mira na sua posição atual e vai direto ao ponto. É o comportamento base para os outros mas é um comportamento burro aonde o NPC apenas vai atras do player e fica sempre atrasado em relaçao a movimentaçao do jogador.

### Flee
O oposto do Seek. Ao jogador entrar em um Raio de panico do NPC ele corre para direçao oposta ao jogador. 

### Arrival
Parecido com o Seek, mas com controle. Faz com que o NPC siga o jogador como no seek mas ao chegar a uma certa distancia começa a desacelerar até parar completamente ao chegar no jogador ou alvo (neste caso jogador).

### Wander
O NPC não tem destino. O NPC tem um circulo na sua frente aonde a cada frame ele escolhe uma direçao aleatória nesse circulo e anda fazzendo parecer um comportamento natural

### Pursuit
A versão inteligente do Seek. ao invés de seguir fielmente o jogador e estar sempre atrasado ele preve aonde o jogador vai com e tenta chegar antes do jogador, fazzendo isso através de um fator de prediçao que diz o quão adiantado ele deve ficar em relaçao ao movimento do jogador.


---

## Como jogar

1. Abra a cena `Hub` e pressione **Play**
2. Mova o personagem com **WASD** ou as **setas do teclado**
3. Chegue perto de uma porta e pressione **E** para entrar na sala
4. Para voltar ao Hub, vá até a porta no canto da sala e pressione **E**

Cada sala tem um NPC com um comportamento diferente. Não tem ordem certa — explore
como quiser.

---

## Estrutura do projeto
```
Assets/
├── Scenes/
│   ├── Main.unity           ← ponto de partida
│   ├── Seek.unity
│   ├── Flee.unity
│   ├── Arrival.unity
│   ├── Wander.unity
│   └── Pursuit.unity
│
└── Scripts/
    ├── PlayerController.cs ← movimento do jogador
    ├── RoomTransition.cs   ← sistema de troca de salas
    ├── PlayerHUD.cs        ← prompt de interação na tela
    ├── SeekBehaviour.cs
    ├── FleeBehaviour.cs
    ├── ArrivalBehaviour.cs
    ├── WanderBehaviour.cs
    └── PursuitBehaviour.cs
```

---

## Problemas que apareceram no caminho

**O NPC de Arrival ficava empurrando o jogador** — O cálculo de parada usava distância
entre centros dos objetos, mas os colliders se tocavam antes disso. Resolvi usando
`OnCollisionEnter2D` para detectar o contato real e zerar a velocidade no momento
exato, em vez de tentar adivinhar a distância certa.

**O NPC de Wander travava nas paredes** — Quando batia, a velocidade zerava e ele
perdia a direção de referência, ficando parado. Resolvi guardando a última direção
válida em uma variável separada. Ao colidir, inverte essa direção e rotaciona o ângulo
de wander fazzendo com que ele se afaste da parede como se tivesse batido e voltado.

---

## Referências

- Reynolds, C. W. (1999). *Steering Behaviors For Autonomous Characters*. Proceedings
  of Game Developers Conference 1999. Miller Freeman Game Group, pp. 763–782.
- Millington, I. (2019). *AI for Games*, 3rd ed. CRC Press, pp. 41–194.
  https://doi.org/10.1201/9781351053303
- Unity Documentation — Rigidbody2D:
  https://docs.unity3d.com/Manual/class-Rigidbody2D.html
- Unity Documentation — SceneManager:
  https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.html