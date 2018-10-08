const signalR = require('@aspnet/signalr')
const axios = require('axios')
const urlBase = "https://githubwebhookfunction.azurewebsites.net/api/"
const axiosConfig = {}

XMLHttpRequest = require("xmlhttprequest").XMLHttpRequest
WebSocket = require("websocket").w3cwebsocket

axios
    .post(urlBase + 'SignalRInfo', null, axiosConfig)
    .then(function(resp) { 

        const options = {
            accessTokenFactory: function() {
                return resp.data.accessToken
            }
        }

        const connection = 
            new signalR.HubConnectionBuilder()
                        .withUrl(resp.data.url, options)
                        .build()

        connection.on('pushReceived', (msg) => {
            console.log('push received  ')
        })

        connection.on('pullRequestOpened', (args) => {
            console.log('pull request opened')
        })

        connection.start()
            .then(() => {
                console.log('the signalr connection has been made')
            })
            .catch((r) => {
                console.log('the signalr connection has failed')
                console.log(r)
            })
    })