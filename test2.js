const { RTMClient } = require('@slack/client');
var botToken = 'xoxb-450052941120-450980774325-McEQJd1Pcivpkp7q5Ytgw9OI';
const rtm = new RTMClient(botToken);
rtm.start();

rtm.on('message', (event) => {
   //if(event.channel == 'CD8QBSDTL')
       console.log(event);
       //rtm.sendMessage('Hello, other person!2','DD81NF9MW');
       // Returns a promise that resolves when the message is sent

});
rtm.sendMessage('123', 'CD8QBSDTL');
