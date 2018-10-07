var axios = require('axios');
const { RTMClient } = require('@slack/client');
const fs = require('fs');
const readline = require('readline');
const {google} = require('googleapis');
const SCOPES = ['https://www.googleapis.com/auth/calendar.readonly'];
const TOKEN_PATH = 'token.json';
var counter = 0;

var key;
var botToken = 'xoxb-450052941120-450980774325-McEQJd1Pcivpkp7q5Ytgw9OI';
const rtm = new RTMClient(botToken);
let personTwoId = '-1';
let personOneId = '-1';
let personTwoChannel = '-1';
let personOneChannel = '-1';
let scheduleAccepted = false;

rtm.start();
rtm.on('message', (event) => {
	if(scheduleAccepted) {
		if(event.user == personTwoId && !event.bot_id) {
			calendar(event.text);
		}
		else if(event.user == personOneId && !event.bot_id) {
			calendar(event.text);
		}
	}
	else if (!event.bot_id && event.user == personTwoId) {
		if (event.text.toUpperCase() == 'ACCEPT'){
			console.log('accepted');
			rtm.sendMessage('They accepted! Time to schedule! follow this link: https://accounts.google.com/o/oauth2/v2/auth?access_type=offline&scope=https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fcalendar.readonly&response_type=code&client_id=205886349566-mdbos4ramaaug4jehfghvmkahc9odcn8.apps.googleusercontent.com&redirect_uri=urn%3Aietf%3Awg%3Aoauth%3A2.0%3Aoob\n Please input the code that you receive from the link:', personOneChannel);
			rtm.sendMessage('You accepted! Time to schedule! follow this link: https://accounts.google.com/o/oauth2/v2/auth?access_type=offline&scope=https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fcalendar.readonly&response_type=code&client_id=205886349566-mdbos4ramaaug4jehfghvmkahc9odcn8.apps.googleusercontent.com&redirect_uri=urn%3Aietf%3Awg%3Aoauth%3A2.0%3Aoob\n Please input the code that you receive from the link:', personTwoChannel);
			scheduleAccepted = true;
		}
		else if (event.text.toUpperCase() == 'DECLINE') {
			console.log('declined');
			rtm.sendMessage('They declined to schedule. Maybe next time!', personOneChannel);
			rtm.sendMessage('You have successfully declined the invitation.', personTwoChannel);
		}
	}
	else if(event.channel != 'CD81JTRHN' && !event.bot_id) {
		rtm.sendMessage('Waiting for ' + event.text.split(' ')[1] + ' to respond...', event.channel);
		if(event.text.includes('!schedule')) {
			let tempName = event.text.split(' ');
			let name =  '';
			personOneId = event.user;
			personOneChannel = event.channel;
			if (tempName.length == 2)
				name = tempName[1];
			else if (tempName.length == 3)
				name = tempName[1] + ' ' + tempName[2];
			getUsers(name, event.user);
		}
	}
})

function getUsers(user, userID) {
	let realName = '';
	axios.get('https://slack.com/api/rtm.start?token=' + botToken)
	.then((res) => {
		for (let tmpUser in res.data.users) {
            if (res.data.users[tmpUser].id == userID)
                realName = res.data.users[tmpUser].real_name;
        }
		for (let tempUser in res.data.users) {
			if (res.data.users[tempUser].real_name == user) {
				axios.get('https://slack.com/api/conversations.list?token=' + botToken + '&scope=bot&types=im')
				.then((response) => {
					for (let tempId in response.data.channels) {
						if (response.data.channels[tempId].user == res.data.users[tempUser].id){
							personTwoId = res.data.users[tempUser].id;
							rtm.sendMessage('Hello ' + user + '!\n' + realName + 'would like to schedule a meeting with you!\n\n Enter "accept" to continue or "decline" to reject their offer.', response.data.channels[tempId].id);
							personTwoChannel = response.data.channels[tempId].id;
						}
					}
				})
			}
		}
	})
	.catch((err) => {
		console.log('Error: ' + err);
	});
}

function calendar(personKey) {
	key = personKey;
	fs.readFile('credentials.json', (err, content) => {
		if (err) return console.log('Error loading client secret file:', err);
		authorize(JSON.parse(content), listEvents);
	});
}

function authorize(credentials, callback) {
	const {client_secret, client_id, redirect_uris} = credentials.installed;
	const oAuth2Client = new google.auth.OAuth2(client_id, client_secret, redirect_uris[0]);
	fs.readFile(TOKEN_PATH, (err, token) => {
		if (err) return getAccessToken(oAuth2Client, callback);
		oAuth2Client.setCredentials(JSON.parse(token));
	callback(oAuth2Client);
	});
}

function getAccessToken(oAuth2Client, callback) {
  const authUrl = oAuth2Client.generateAuthUrl({
    access_type: 'offline',
    scope: SCOPES,
  });
  console.log('Authorize this app by visiting this url:', authUrl);
  const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout,
  });
    oAuth2Client.getToken(key, (err, token) => {
        if (err) return console.error('Error retrieving access token', err);
				oAuth2Client.setCredentials(token);
				//Store the token to disk for later program executions
        callback(oAuth2Client);
    });
}

function listEvents(auth) {
  const calendar = google.calendar({version: 'v3', auth});
  var today = new Date();
  today.setTime(today.getTime() + 10 *  86400000 );
  calendar.events.list({
    calendarId: 'primary',
    timeMin: (new Date()).toISOString(),
    timeMax: today.toISOString(),
    maxResults: 30,
    singleEvents: true,
    orderBy: 'startTime',
  }, (err, res) => {
    if (err) return console.log('The API returned an error: ' + err);
		const events = res.data.items;
		person = '';
    if (events.length) {
      events.map((event, i) => {
        const start = event.start.dateTime || event.start.date;
        const end = event.end.dateTime || event.end.date;
        var startS = `${start}`;
				var endS = `${end}`;
				person += startS + ' ' + endS + '\n';
				console.log(startS +' '+ endS + `${event.summary}`);
			});
			fs.writeFile('events' + ++counter + '.txt', person, listEvents), (err) => {
				if (err) console.error(err);
				console.log('Events stored to', 'events.txt');
			}

			fs.close();
    } else {
      console.log('No upcoming events found.');
    }
  });
}