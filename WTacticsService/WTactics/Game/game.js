Vue.directive('animatemovecard', {
    bind: function () {
    },
    update: function (value) {
        if (value.animateCardMove) {
            $(this.el).addClass("animatemovecard").one("transitionend MSTransitionEnd webkitTransitionEnd oTransitionEnd", function () {
                value.item.animateCardMove = false;
            });
        }
        else {
            $(this.el).removeClass("animatemovecard");
        }
    },
    unbind: function () {
    },
});


/* The vue app, containing the visualisation, and the ui actions */
var vue = new Vue({
    el: '#content',
    data: {
        showModal: false,
        transformMatrix: {},
        inverseTransformMatrix: {},
        screenTransformMatrix: {},
        inverseScreenTransformMatrix: {},
        perspectiveDragOriginLeft: 0,
        perspectiveDragOriginTop: 0,
        perspectiveDragCssPostion: 'relative',
        previewImageSrc: "",
        preview: false,
        gameGuid: null,
        isStarted: false,
        cards: [],
        player: {
            playerGuid: null,
            deckGuid: null,
            statsTimer: null,
            deck: [],
            play: [],
            hand: [],
            removed: [],
            graveyard: [],
            VictoryPoints: 15,
            resources: {
                black: { used: 0, available: 0 },
                yellow: { used: 0, available: 0 },
                green: { used: 0, available: 0 },
                blue: { used: 0, available: 0 },
                red: { used: 0, available: 0 },
            }
        },
        opponent: {
            playerGuid: null,
            deckGuid: null,
            statsTimer: null,
            deck: [],
            play: [],
            hand: [],
            removed: [],
            graveyard: [],
            VictoryPoints: 15,
            resources: {
                black: { used: 0, available: 0 },
                yellow: { used: 0, available: 0 },
                green: { used: 0, available: 0 },
                blue: { used: 0, available: 0 },
                red: { used: 0, available: 0 },
            }
        },
        cardlist: {
            show: false,
            cards: [],
            sync: {
                playerGuid: null,
                kind: null,
                mustsync: false,
            },
            oldIndex: -1,
            selectedCard: null,
        },
        services: {
            login: null,
            logout: null,
            users: null,
            cards: null,
            cardSearch: null,
            decks: null,
            deckSearch: null,
            deckCards: null,
            cardOptions: null,
            gameSearch: null,
            games: null,
            game: null,
        }

    },
    ready: function () {
        // create rest services
        this.createServices();
        // Add watcher for when the modal dialog closes,
        // - syncs the card visible state
        this.$watch('cardlist.show', function (value) {
            if (!value) {
                vue.syncCardList();
                setDroppableState(true);
            }
        });
        // Apply the layouting (matrix-3d transform, position victory point sliders)
        $(window).on('resize', function (e) { resizeGame(); }, 1000).resize();
        // Set up the droppable regions on the battlefield
        setupDropRegions();

        // Start the game's BL
        $(init);
    },
    methods: {
        createServices: function () {
            this.services.login = this.$resource("/api/Login");
            this.services.logout = this.$resource("/api/Logout");
            this.services.users = this.$resource("/api/Users{/guid}");
            this.services.cards = this.$resource("/api/Cards{/guid}");
            this.services.cardSearch = this.$resource("/api/CardSearch");
            this.services.decks = this.$resource("/api/Decks{/guid}");
            this.services.deckSearch = this.$resource("/api/DeckSearch");
            this.services.deckCards = this.$resource("/api/DeckCards");
            this.services.cardOptions = this.$resource("/api/CardOptions{/guid}");
            this.services.gameSearch = this.$resource("/api/GameSearch");
            this.services.games = this.$resource("/api/Games");
            this.services.game = this.$resource("/api/Game{/gameGuid}{/playerGuid}");
        },
        toggleMark: function (card) {
            sendGameAction({
                gameGuid: vue.gameGuid,
                playerGuid: vue.player.playerGuid,
                actionType: 'changeCardState',
                actionData: {
                    cardId: card.cardId,
                    isMarked: !card.isMarked,
                }
            });
        },
        toggleFaceDown: function (card) {
            sendGameAction({
                gameGuid: vue.gameGuid,
                playerGuid: vue.player.playerGuid,
                actionType: 'changeCardState',
                actionData: {
                    cardId: card.cardId,
                    isFaceDown: !card.isFaceDown,
                }
            });
        },
        increaseCounter: function (card, kind) {
            if (kind === 'counterA') {
                sendGameAction({
                    gameGuid: vue.gameGuid,
                    playerGuid: vue.player.playerGuid,
                    actionType: 'changeCardState',
                    actionData: {
                        cardId: card.cardId,
                        counterA: card.counterA + 1,
                    }
                });
            }
            if (kind === 'counterB') {
                sendGameAction({
                    gameGuid: vue.gameGuid,
                    playerGuid: vue.player.playerGuid,
                    actionType: 'changeCardState',
                    actionData: {
                        cardId: card.cardId,
                        counterB: card.counterB + 1,
                    }
                });
            }
        },
        decreaseCounter: function (card, kind) {
            if (kind === 'counterA') {
                var newCounterA = card.counterA - 1;
                if (newCounterA < 0) newCounterA = 0;
                sendGameAction({
                    gameGuid: vue.gameGuid,
                    playerGuid: vue.player.playerGuid,
                    actionType: 'changeCardState',
                    actionData: {
                        cardId: card.cardId,
                        counterA: newCounterA,
                    }
                });
            }
            if (kind === 'counterB') {
                var newCounterB = card.counterB - 1;
                if (newCounterB < 0) newCounterB = 0;
                sendGameAction({
                    gameGuid: vue.gameGuid,
                    playerGuid: vue.player.playerGuid,
                    actionType: 'changeCardState',
                    actionData: {
                        cardId: card.cardId,
                        counterB: newCounterB,
                    }
                });
            }
        },
        counterWheel: function (event, card, kind) {
            var e = window.event || event; // old IE support
            var delta = Math.max(-1, Math.min(1, (e.wheelDelta || -e.detail)));
            if (delta > 0) {
                this.increaseCounter(card, kind);
            } else {
                this.decreaseCounter(card, kind);
            };
            return false;
        },
        moveCardFrom: function (actionType, fromPlayerGuid, fromKind, toPlayerGuid, toKind, isFaceDown) {
            sendGameAction({
                gameGuid: vue.gameGuid,
                playerGuid: vue.player.playerGuid,
                actionType: actionType,
                actionData: {
                    fromPlayerGuid: fromPlayerGuid,
                    fromKind: fromKind,
                    toPlayerGuid: toPlayerGuid,
                    toKind: toKind,
                    cardState: {
                        isFaceDown: isFaceDown,
                    }
                }
            });
        },
        showPreview: function (card) {
            if (!card.isFaceDown) {
                this.previewImageSrc = card.imageSrc;
                this.preview = true;
            }
            else {
                this.preview = false;
            }
        },
        hidePreview: function (card) {
            this.preview = false;
        },
        increaseAvailableResource: function (playerGuid, resource) {
            if (resource.available < 99) {
                resource.available++;
                updatePlayerStatsAction(playerGuid);
            }
        },
        decreaseAvailableResource: function (playerGuid, resource) {
            if (resource.available > 0) {
                resource.available--;
                updatePlayerStatsAction(playerGuid);
            }
        },
        availableResourceWheel: function (event, playerGuid, resource) {
            var e = window.event || event; // old IE support
            var delta = Math.max(-1, Math.min(1, (e.wheelDelta || -e.detail)));
            if (delta > 0) {
                this.increaseAvailableResource(playerGuid, resource);

            } else {
                this.decreaseAvailableResource(playerGuid, resource);
            };
            return false;
        },
        increaseUsedResource: function (playerGuid, resource) {
            if (resource.used < resource.available) {
                resource.used++;
                updatePlayerStatsAction(playerGuid);
            }
        },
        decreaseUsedResource: function (playerGuid, resource) {
            if (resource.used > 0) {
                resource.used--;
                updatePlayerStatsAction(playerGuid);
            }
        },
        usedResourceWheel: function (event, playerGuid, resource) {
            var e = window.event || event; // old IE support
            var delta = Math.max(-1, Math.min(1, (e.wheelDelta || -e.detail)));
            if (delta > 0) {
                this.increaseUsedResource(playerGuid, resource);
            } else {
                this.decreaseUsedResource(playerGuid, resource);
            };
            return false;
        },
        shuffleCards: function (playerGuid, kind) {
            sendGameAction({
                gameGuid: vue.gameGuid,
                playerGuid: vue.player.playerGuid,
                actionType: 'shuffleList',
                actionData: {
                    playerGuid: playerGuid,
                    kind: kind,
                }
            });
        },

        showCardList: function (playerGuid, kind) {

            setDroppableState(false);
            this.cardlist.sync.mustsync = true;
            this.cardlist.cards = [];
            this.cardlist.sync.playerGuid = playerGuid;
            this.cardlist.sync.kind = kind;
            this.cardlist.selectedCard = null;
            var isFaceDown = kind !== 'graveyard';
            var list = getList(playerGuid, kind);
            $.each(list, function (index, card) {
                vue.cardlist.cards.push({
                    cardId: card.cardId,
                    imageSrc: card.imageSrc,
                    isFaceDown: isFaceDown,
                });
            });
            this.cardlist.cards.reverse();

            $("#cardlist").sortable({
                start: function (event, ui) {
                    vue.cardlist.oldIndex = ui.item.index();
                },
                update: function (event, ui) {
                    var newIndex = ui.item.index();
                    vue.swapCards(vue.cardlist.cards, vue.cardlist.oldIndex, newIndex);
                    vue.cardlist.oldIndex = newIndex;
                },
            });
            this.cardlist.show = true;
        },
        syncCardList: function () {
            if (!this.cardlist.sync.mustsync) return;
            var actionData = {
                playerGuid: this.cardlist.sync.playerGuid,
                kind: this.cardlist.sync.kind,
                cards: []
            };

            this.cardlist.cards.reverse();

            $.each(this.cardlist.cards, function (index, card) {
                actionData.cards.push({
                    cardId: card.cardId,
                    isFaceDown: card.isFaceDown
                });
            });
            sendGameAction({
                gameGuid: vue.gameGuid,
                playerGuid: vue.player.playerGuid,
                actionType: 'updateList',
                actionData: actionData
            });
            
        },
        selectFromCardList(card) {
            this.cardlist.selectedCard = card;
        },
        setCardListFaceDown(faceDown) {
            $.each(this.cardlist.cards, function (index, card) {
                card.isFaceDown = faceDown;
            });
        },
        shuffleCardList: function () {
            var currentIndex = this.cardlist.cards.length, randomIndex;
            while (0 !== currentIndex) {
                randomIndex = Math.floor(Math.random() * currentIndex);
                currentIndex -= 1;
                this.swapCards(this.cardlist.cards, currentIndex, randomIndex);
            }
        },
        swapCards: function (list, oldIndex, newIndex) {
            list.splice(newIndex, 0, list.splice(oldIndex, 1)[0]);
        },
        toggleCardListFaceDown: function(card) {
            card.isFaceDown = !card.isFaceDown;
        },
        cardlistDrawCard: function () {

            if (this.cardlist.selectedCard) {
                if (!(this.cardlist.sync.kind === 'hand' &&
                    this.cardlist.sync.playerGuid === vue.player.playerGuid)) {
                    sendGameAction({
                        gameGuid: vue.gameGuid,
                        playerGuid: vue.player.playerGuid,
                        actionType: 'drawCard',
                        actionData: {
                            fromPlayerGuid: this.cardlist.sync.playerGuid,
                            fromKind: this.cardlist.sync.kind,
                            toPlayerGuid: vue.player.playerGuid,
                            toKind: 'hand',
                            cardId: this.cardlist.selectedCard.cardId,
                            cardState: {
                                cardId: this.cardlist.selectedCard.cardId,
                                isFaceDown: this.cardlist.selectedCard.isFaceDown,
                            }
                        }
                    });
                    this.cardlist.sync.mustsync = false;
                    this.cardlist.show = false;
                }
            }
        },
        cardlistDiscardCard: function () {
            if (this.cardlist.selectedCard) {
                if (this.cardlist.sync.kind !== 'graveyard') {
                    sendGameAction({
                        gameGuid: vue.gameGuid,
                        playerGuid: vue.player.playerGuid,
                        actionType: 'discardCard',
                        actionData: {
                            fromPlayerGuid: this.cardlist.sync.playerGuid,
                            fromKind: this.cardlist.sync.kind,
                            toPlayerGuid: this.cardlist.sync.playerGuid,
                            toKind: 'graveyard',
                            cardId: this.cardlist.selectedCard.cardId,
                            cardState: {
                                cardId: this.cardlist.selectedCard.cardId,
                                isFaceDown: false,
                            }
                        }
                    });
                    this.cardlist.sync.mustsync = false;
                    this.cardlist.show = false;
                }
            }
        },
        cardlistPlayCard: function () {
            if (this.cardlist.selectedCard) {
                if (this.cardlist.sync.kind !== 'play') {
                    sendGameAction({
                        gameGuid: vue.gameGuid,
                        playerGuid: vue.player.playerGuid,
                        actionType: 'playCard',
                        actionData: {
                            fromPlayerGuid: this.cardlist.sync.playerGuid,
                            fromKind: this.cardlist.sync.kind,
                            toPlayerGuid: this.cardlist.sync.playerGuid,
                            toKind: 'play',
                            cardId: this.cardlist.selectedCard.cardId,
                            cardState: {
                                cardId: this.cardlist.selectedCard.cardId,
                                isFaceDown: this.cardlist.selectedCard.isFaceDow,
                            }
                        }
                    });
                    this.cardlist.sync.mustsync = false;
                    this.cardlist.show = false;
                }
            }
        },
        cardlistDeckCard: function () {
            if (this.cardlist.selectedCard) {
                if (this.cardlist.sync.kind !== 'deck') {
                    sendGameAction({
                        gameGuid: vue.gameGuid,
                        playerGuid: vue.player.playerGuid,
                        actionType: 'deckCard',
                        actionData: {
                            fromPlayerGuid: this.cardlist.sync.playerGuid,
                            fromKind: this.cardlist.sync.kind,
                            toPlayerGuid: this.cardlist.sync.playerGuid,
                            toKind: 'deck',
                            cardId: this.cardlist.selectedCard.cardId,
                            cardState: {
                                cardId: this.cardlist.selectedCard.cardId,
                                isFaceDown: this.cardlist.selectedCard.isFaceDow,
                            }
                        }
                    });
                    this.cardlist.sync.mustsync = false;
                    this.cardlist.show = false;
                }
            }
        },
       
    },
});

/* Process game actions*/
function processGameAction(gameAction) {
    console.log(JSON.stringify(gameAction, null, 2));
    switch (gameAction.actionType) {
        case 'joinGame':
            break;
        case 'startGame':
            processStartGame(gameAction.actionResult);
            break;
        case 'drawCard':
        case 'discardCard':
        case 'playCard':
        case 'deckCard':
        case 'removeCard':
            if(vue.isStarted) processMoveCard(gameAction.actionData);
            break;
        case 'changeCardState':
            if (vue.isStarted) {
                var animate = gameAction.playerGuid === vue.opponent.playerGuid;
                processChangeCardState(gameAction.actionData, animate);
            }
            break;
        case 'changePlayerStats':
            if(vue.isStarted) processChangePlayerStats(gameAction.actionData);
            break;
        case 'shuffleList':
            if (vue.isStarted) processShuffleList(gameAction.actionData, gameAction.actionResult);
            break;
        case 'updateList':
            if (vue.isStarted) processUpdateList(gameAction.actionData, gameAction.actionResult);
            break;
        case 'leaveGame':
            if (vue.isStarted) {
               
                processLeave(gameAction);
            }
            break;

    }
}

function processLeave(gameAction) {
    var mustShow = gameAction.playerGuid === vue.opponent.playerGuid;
    if (mustShow) {
        alert("Opponent left the game");
    }

}

/* Game bootstrap */
function init() {
    /* get the game id from the url */
    vue.gameGuid = $.urlParam('gameGuid');
    /* get the player id from the url */
    vue.player.playerGuid = $.urlParam('playerGuid');
    /* get the deck id from the url */
    vue.player.deckGuid = $.urlParam('deckGuid');

    window.onbeforeunload = function (e) {
        sendGameAction({
            gameGuid: vue.gameGuid,
            playerGuid: vue.player.playerGuid,
            actionType: 'leaveGame',
        });
        $.connection.hub.stop();
    };

    $.connection.hub.url = "http://" + window.location.hostname + ":9091/signalr";

    /* set up the web push api, and send the join game action on completion */
    /* when the join is successful, the processGameAction callback is called every time an action happens in the game */
    /* using the sendGameAction method, a action can be triggered on all clients */
    /* setup callback for game actions*/
    $.connection.games.client.processAction = processGameAction;
    /* open communications hub and join game when it is up */
    $.connection.hub.logging = true;
    $.connection.hub.start().done(joinGame);
}

/* trigger game action on all clients */
function sendGameAction(gameAction) {
    $.connection.games.server.pushAction(gameAction);
}

/* join the game */
function joinGame() {
    $.connection.games.server.joinGame(vue.gameGuid, vue.player.playerGuid)
        .done(function() {
            sendGameAction({
                gameGuid: vue.gameGuid,
                playerGuid: vue.player.playerGuid,
                actionType: 'loadDeck',
                actionData: { deckGuid: vue.player.deckGuid }
            });
        });
}



/* Region: game actions */

/* Action game start */
function loadDeck(source, target) {
    var imageUrlBase = "http://wtactics.westeurope.cloudapp.azure.com";
    $.each(source.cards, function (index, card) {
        var c = {
            cardId: card.id,
            imageSrc: imageUrlBase + card.url,
            isMarked: card.isMarked,
            isDraggable: card.isDraggable,
            isFaceDown: card.isFaceDown,
            top: card.top,
            left: card.left,
            counterA: card.counterA,
            counterB: card.counterB,
            animateCardMove: false,
        };
        vue.cards.push(c);
        target.push(c);
    });
}

function processStartGame(game) {
    var player = game.players.find(function (element) {
        return element.playerGuid === vue.player.playerGuid;
    });
   
    loadDeck(player.deck, vue.player.deck);


    var opponent = game.players.find(function (element) {
        return element.playerGuid !== vue.player.playerGuid;
    });

    vue.opponent.playerGuid = opponent.playerGuid;
    loadDeck(opponent.deck, vue.opponent.deck);

    setTimeout(function () {
        $("#player .full .rs-tooltip").css('background-image', 'url(' + player.avatar + ')');
        $("#opponent .full .rs-tooltip").css('background-image', 'url(' + opponent.avatar + ')');
    }, 1500);

    vue.isStarted = true;
}

/* Action move card form one list to another */
function processMoveCard(moveCardParam) {
    /* Nothing to do if the move destination is the same as the source*/
    if (moveCardParam.fromPlayerGuid === moveCardParam.toPlayerGuid &&
        moveCardParam.fromKind === moveCardParam.toKind) return;

    var source = getList(moveCardParam.fromPlayerGuid, moveCardParam.fromKind);
    var card;
    if (moveCardParam.cardId !== undefined) {
        card = source.find(function (element) {
            return element.cardId === moveCardParam.cardId;
        });
        source.$remove(card);
    } else {
        card = source.pop();
    }
    if (card) {
        var target = getList(moveCardParam.toPlayerGuid, moveCardParam.toKind);
        target.push(card);
        if (moveCardParam.cardState !== undefined) {
            moveCardParam.cardState.cardId = card.cardId;
            processChangeCardState(moveCardParam.cardState, false);
        }
    }
}

/* Action update a card's state (location, mark/unmark, faceUp/faceDown) */
function updateCardLocation(card, top, left, animate) {

    card.animateCardMove = animate;
  

    var isPlayerCard = vue.player.play.find(function (element) {
        return element.cardId === card.cardId;
    });
    if (isPlayerCard !== undefined) {
        card.top = top;
        card.left = left;
    }
    var isOpponentCard = vue.opponent.play.find(function (element) {
        return element.cardId === card.cardId;
    });
    /* mirror the location using the battlefield line as mirroring line if it's an opponent card */
    if (isOpponentCard !== undefined) {
        card.top = 1200 - top - 150;
        card.left = left;
    }
    if (isOpponentCard === undefined && isPlayerCard === undefined) {
        card.top = 0;
        card.left = 0;
    }
  
}

function processChangeCardState(state, animate) {
    var card = vue.cards.find(function (element) {
        return element.cardId === state.cardId;
    });
    if (card) {
        if (state.isMarked !== undefined) card.isMarked = state.isMarked;
        if (state.isDraggable !== undefined) card.isDraggable = state.isDraggable;
        if (state.isFaceDown !== undefined) card.isFaceDown = state.isFaceDown;
        if (state.counterA !== undefined) card.counterA = state.counterA;
        if (state.counterB !== undefined) card.counterB = state.counterB;
        if (state.top !== undefined && state.left !== undefined) {
            updateCardLocation(card, state.top, state.left, animate);
        }
    }
}

/* Action update a player's stats (victory points, resources) */

/* updatePlayerStatsAction is a delayed triggered action, to bundle fast changes to 
  the resources/victory points of the player, before sending the action to all clients */
function updatePlayerStatsAction(playerGuid) {
    var player = getPlayer(playerGuid);
    if (player.statsTimer) {
        clearTimeout(player.statsTimer);
    }
    player.statsTimer = setTimeout(function () {
        sendGameAction({
            gameGuid: vue.gameGuid,
            playerGuid: vue.player.playerGuid,
            actionType: 'changePlayerStats',
            actionData: {
                playerGuid: player.playerGuid,
                victoryPoints: player.VictoryPoints,
                resources: {
                    black: {
                        used: player.resources.black.used,
                        available: player.resources.black.available,
                    },
                    blue: {
                        used: player.resources.blue.used,
                        available: player.resources.blue.available,
                    },
                    red: {
                        used: player.resources.red.used,
                        available: player.resources.red.available,
                    },
                    green: {
                        used: player.resources.green.used,
                        available: player.resources.green.available,
                    },
                    yellow: {
                        used: player.resources.yellow.used,
                        available: player.resources.yellow.available,
                    }
                }
            }
        });
    }, 1500);
}

function processChangePlayerStats(playerState) {
    var player = getPlayer(playerState.playerGuid);
    if (player.VictoryPoints !== playerState.victoryPoints) {
        player.VictoryPoints = playerState.victoryPoints;
        if (vue.player.playerGuid === playerState.playerGuid) {
            $("#playerVicoryPoints").roundSlider({
                value: player.VictoryPoints
            });
        }
        if (vue.opponent.playerGuid === playerState.playerGuid) {
            $("#opponentVicoryPoints").roundSlider({
                value: player.VictoryPoints
            });
        }
    }
    player.resources.black.available = playerState.resources.black.available;
    player.resources.black.used = playerState.resources.black.used;
    player.resources.red.available = playerState.resources.red.available;
    player.resources.red.used = playerState.resources.red.used;
    player.resources.blue.available = playerState.resources.blue.available;
    player.resources.blue.used = playerState.resources.blue.used;
    player.resources.green.available = playerState.resources.green.available;
    player.resources.green.used = playerState.resources.green.used;
    player.resources.yellow.available = playerState.resources.yellow.available;
    player.resources.yellow.used = playerState.resources.yellow.used;
}

function processShuffleList(shuffleListParam, gamecards) {
    
    clearList(shuffleListParam.playerGuid, shuffleListParam.kind);
    var source = getList(shuffleListParam.playerGuid, shuffleListParam.kind);
    
    $.each(gamecards, function (index, gamecard) {
        var card = vue.cards.find(function (element) {
            return element.cardId === gamecard.id;
        });
        if (card !== undefined) {
            source.push(card);
        }
    });
}

function processUpdateList(updateListParam, gamecards) {

    clearList(updateListParam.playerGuid, updateListParam.kind);
    var source = getList(updateListParam.playerGuid, updateListParam.kind);

    $.each(gamecards, function (index, gamecard) {
        var card = vue.cards.find(function (element) {
            return element.cardId === gamecard.id;
        });
        if (card !== undefined && card != null) {
            card.isFaceDown = gamecard.isFaceDown;
            card.isMarked = gamecard.isMarked;
            card.isDraggable = gamecard.isDraggable;
            source.push(card);
        }
    });
}

/* Region: helpers */
function getPlayer(playerGuid) {
    if (vue.player.playerGuid === playerGuid) return vue.player;
    if (vue.opponent.playerGuid === playerGuid) return vue.opponent;
    return null;
}

function clearList(playerGuid, kind) {
    var player = getPlayer(playerGuid);
    if (player) {
        switch (kind) {
            case 'deck':
                player.deck = [];
                break;
            case 'graveyard':
                player.graveyard = [];
                break;
            case 'hand':
                player.hand = [];
                break;
            case 'play':
                player.play = [];
                break;
            case 'removed':
                player.removed = [];
                break;
        }
    }
}

function getList(playerGuid, kind) {
    var player = getPlayer(playerGuid);
    if (!player) return null;
    switch (kind) {
        case 'deck':
            return player.deck;
        case 'graveyard':
            return player.graveyard;
        case 'hand':
            return player.hand;
        case 'play':
            return player.play;
        case 'removed':
            return player.removed;
        default:
            return null;
    }
}
/* EndRegion: helpers*/

/* EndRegion: game actions */

/* Region: droppables */

function setDroppableState(isEnabled) {
    var state = isEnabled ? "enable" : "disable";
    $("#battleField").droppable(state);
    $("#playerHand").droppable(state);
    $("#playerDeck").droppable(state);
    $("#playerGraveyard").droppable(state);
    $("#opponentHand").droppable(state);
    $("#opponentDeck").droppable(state);
    $("#opponentGraveyard").droppable(state);
}

function setupDropRegions() {
    // Define playerhand as a drop target, when a card it dropped, change it in the datastructures
    $("#playerHand").droppable({
        classes: {
            "ui-droppable-hover": "droptarget"
        },
        // greedy doesn't work for siblings
        // greedy: true,
        tolerance: 'perspectiveintersect',
        drop: function (event, ui) {
            var dragdata = $(ui.helper).data('dragdata');

            if (!(dragdata.fromPlayerGuid === vue.player.playerGuid && dragdata.fromKind === 'hand')) {
                sendGameAction({
                    gameGuid: vue.gameGuid,
                    playerGuid: vue.player.playerGuid,
                    actionType: 'drawCard',
                    actionData: {
                        fromPlayerGuid: dragdata.fromPlayerGuid,
                        fromKind: dragdata.fromKind,
                        toPlayerGuid: vue.player.playerGuid,
                        toKind: 'hand',
                        cardId: dragdata.item.cardId,
                        cardState: {
                            cardId: dragdata.item.cardId,
                            isFaceDown: false,
                            top: 0,
                            left: 0
                        }
                    }
                });

            } else {
                dragdata.top = 0;
                dragdata.left = 0;
                dragdata.item.top = 0;
                dragdata.item.left = 0;
                $(ui.helper).css({ top: 0, left: 0 });
            }
            return true;
        },
        // disable battelfield drop
        over: function (event, ui) {
            $("#battleField").droppable("disable");
        },
        // enable battelfield drop
        out: function (event, ui) {
            $("#battleField").droppable("enable");
        }
    });

    // Define opponenthand as a drop target, when a card it dropped, change it in the datastructures
    $("#opponentHand").droppable({
        classes: {
            "ui-droppable-hover": "droptarget"
        },
        greedy: true,
        tolerance: 'perspectiveintersect',
        drop: function (event, ui) {

            var dragdata = $(ui.helper).data('dragdata');
            if (!(dragdata.fromPlayerGuid === vue.opponent.playerGuid && dragdata.fromKind === 'hand')) {
                $(ui.helper).hide();
                dragdata.top = 0;
                dragdata.left = 0;
                dragdata.item.top = 0;
                dragdata.item.left = 0;
                $(ui.helper).css({ top: 0, left: 0 });
                sendGameAction({
                    gameGuid: vue.gameGuid,
                    playerGuid: vue.player.playerGuid,
                    actionType: 'drawCard',
                    actionData: {
                        fromPlayerGuid: dragdata.fromPlayerGuid,
                        fromKind: dragdata.fromKind,
                        toPlayerGuid: vue.opponent.playerGuid,
                        toKind: 'hand',
                        cardId: dragdata.item.cardId,
                        cardState: {
                            cardId: dragdata.item.cardId,
                            isFaceDown: false,
                            top: 0,
                            left: 0
                        }
                    }
                });

            }

            return true;
        },
        // disable battelfield drop
        over: function (event, ui) {
            $("#battleField").droppable("disable");
        },
        // enable battelfield drop
        out: function (event, ui) {
            $("#battleField").droppable("enable");
        }
    });

    // Define player graveyard as a drop target, when a card it dropped, change it in the datastructures
    $("#playerGraveyard").droppable({
        classes: {
            "ui-droppable-hover": "droptarget"
        },
        greedy: true,
        tolerance: 'perspectiveintersect',
        drop: function (event, ui) {

            var dragdata = $(ui.helper).data('dragdata');
            if (!(dragdata.fromPlayerGuid === vue.player.playerGuid && dragdata.fromKind === 'graveyard')) {
                $(ui.helper).hide();
                dragdata.top = 0;
                dragdata.left = 0;
                dragdata.item.top = 0;
                dragdata.item.left = 0;
                $(ui.helper).css({ top: 0, left: 0 });
                sendGameAction({
                    gameGuid: vue.gameGuid,
                    playerGuid: vue.player.playerGuid,
                    actionType: 'discardCard',
                    actionData: {
                        fromPlayerGuid: dragdata.fromPlayerGuid,
                        fromKind: dragdata.fromKind,
                        toPlayerGuid: vue.player.playerGuid,
                        toKind: 'graveyard',
                        cardId: dragdata.item.cardId,
                        cardState: {
                            cardId: dragdata.item.cardId,
                            isFaceDown: false,
                            top: 0,
                            left: 0
                        }
                    }
                });

            }

            return true;
        },
        // disable battelfield drop
        over: function (event, ui) {
            $("#battleField").droppable("disable");
        },
        // enable battelfield drop
        out: function (event, ui) {
            $("#battleField").droppable("enable");
        }
    });

    // Define player deck as a drop target, when a card it dropped, change it in the datastructures
    $("#playerDeck").droppable({
        classes: {
            "ui-droppable-hover": "droptarget"
        },
        greedy: true,
        tolerance: 'perspectiveintersect',
        drop: function (event, ui) {

            var dragdata = $(ui.helper).data('dragdata');
            if (!(dragdata.fromPlayerGuid === vue.player.playerGuid && dragdata.fromKind === 'deck')) {
                $(ui.helper).hide();
                dragdata.top = 0;
                dragdata.left = 0;
                dragdata.item.top = 0;
                dragdata.item.left = 0;
                $(ui.helper).css({ top: 0, left: 0 });
                sendGameAction({
                    gameGuid: vue.gameGuid,
                    playerGuid: vue.player.playerGuid,
                    actionType: 'deckCard',
                    actionData: {
                        fromPlayerGuid: dragdata.fromPlayerGuid,
                        fromKind: dragdata.fromKind,
                        toPlayerGuid: vue.player.playerGuid,
                        toKind: 'deck',
                        cardId: dragdata.item.cardId,
                        cardState: {
                            cardId: dragdata.item.cardId,
                            isFaceDown: true,
                            top: 0,
                            left: 0
                        }
                    }
                });

            }
            return true;
        },
        // disable battelfield drop
        over: function (event, ui) {
            $("#battleField").droppable("disable");
        },
        // enable battelfield drop
        out: function (event, ui) {
            $("#battleField").droppable("enable");
        }
    });

    // Define opponent graveyard as a drop target, when a card it dropped, change it in the datastructures
    $("#opponentGraveyard").droppable({
        classes: {
            "ui-droppable-hover": "droptarget"
        },
        greedy: true,
        tolerance: 'perspectiveintersect',
        drop: function (event, ui) {

            var dragdata = $(ui.helper).data('dragdata');
            if (!(dragdata.fromPlayerGuid === vue.opponent.playerGuid && dragdata.fromKind === 'graveyard')) {
                $(ui.helper).hide();
                dragdata.top = 0;
                dragdata.left = 0;
                dragdata.item.top = 0;
                dragdata.item.left = 0;
                $(ui.helper).css({ top: 0, left: 0 });
                sendGameAction({
                    gameGuid: vue.gameGuid,
                    playerGuid: vue.player.playerGuid,
                    actionType: 'discardCard',
                    actionData: {
                        fromPlayerGuid: dragdata.fromPlayerGuid,
                        fromKind: dragdata.fromKind,
                        toPlayerGuid: vue.opponent.playerGuid,
                        toKind: 'graveyard',
                        cardId: dragdata.item.cardId,
                        cardState: {
                            cardId: dragdata.item.cardId,
                            isFaceDown: false,
                            top: 0,
                            left: 0
                        }
                    }
                });

            }

            return true;
        },
        // disable battelfield drop
        over: function (event, ui) {
            $("#battleField").droppable("disable");
        },
        // enable battelfield drop
        out: function (event, ui) {
            $("#battleField").droppable("enable");
        }
    });

    // Define opponent deck as a drop target, when a card it dropped, change it in the datastructures
    $("#opponentDeck").droppable({
        classes: {
            "ui-droppable-hover": "droptarget"
        },
        greedy: true,
        tolerance: 'perspectiveintersect',
        drop: function (event, ui) {

            var dragdata = $(ui.helper).data('dragdata');
            if (!(dragdata.fromPlayerGuid === vue.opponent.playerGuid && dragdata.fromKind === 'deck')) {
                $(ui.helper).hide();
                dragdata.top = 0;
                dragdata.left = 0;
                dragdata.item.top = 0;
                dragdata.item.left = 0;
                $(ui.helper).css({ top: 0, left: 0 });
                sendGameAction({
                    gameGuid: vue.gameGuid,
                    playerGuid: vue.player.playerGuid,
                    actionType: 'deckCard',
                    actionData: {
                        fromPlayerGuid: dragdata.fromPlayerGuid,
                        fromKind: dragdata.fromKind,
                        toPlayerGuid: vue.opponent.playerGuid,
                        toKind: 'deck',
                        cardId: dragdata.item.cardId,
                        cardState: {
                            cardId: dragdata.item.cardId,
                            isFaceDown: true,
                            top: 0,
                            left: 0
                        }
                    }
                });

            }
            return true;
        },
        // disable battelfield drop
        over: function (event, ui) {
            $("#battleField").droppable("disable");
        },
        // enable battelfield drop
        out: function (event, ui) {
            $("#battleField").droppable("enable");
        }
    });

    // Define the battelfield as a drop target, when a card it dropped, change it in the datastructures
    $("#battleField").droppable({
        tolerance: 'perspectiveintersect',
        drop: function (event, ui) {
            var dragdata = $(ui.helper).data('dragdata');

            if (dragdata.fromKind !== "play") {
                sendGameAction({
                    gameGuid: vue.gameGuid,
                    playerGuid: vue.player.playerGuid,
                    actionType: 'playCard',
                    actionData: {
                        fromPlayerGuid: dragdata.fromPlayerGuid,
                        fromKind: dragdata.fromKind,
                        toPlayerGuid: vue.player.playerGuid,
                        toKind: 'play',
                        cardId: dragdata.item.cardId,
                        cardState: {
                            cardId: dragdata.item.cardId,
                            isFaceDown: dragdata.item.isFaceDown,
                            top: dragdata.top,
                            left: dragdata.left
                        }
                    }
                });
            } else {
                if (dragdata.fromPlayerGuid === vue.player.playerGuid) {
                    sendGameAction({
                        gameGuid: vue.gameGuid,
                        playerGuid: vue.player.playerGuid,
                        actionType: 'changeCardState',
                        actionData: {
                            cardId: dragdata.item.cardId,
                            top: dragdata.top,
                            left: dragdata.left,
                        }
                    });
                }
                if (dragdata.fromPlayerGuid === vue.opponent.playerGuid) {
                    sendGameAction({
                        gameGuid: vue.gameGuid,
                        playerGuid: vue.player.playerGuid,
                        actionType: 'changeCardState',
                        actionData: {
                            cardId: dragdata.item.cardId,
                            top: 1200 - dragdata.top - 150,
                            left: dragdata.left,
                        }
                    });
                }
            }
            return true;
        }
    });
}

/* EndRegion: droppables */
