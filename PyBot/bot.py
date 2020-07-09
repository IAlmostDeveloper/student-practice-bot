#!/usr/bin/env python3.6
import requests
import sqlite3
import uuid

ACCESS_TOKEN = "e4457ff3d3353f9faccbcaaca9d17d172bf926cf15c9657b336c7c46ecb65b69c3674755905e0335b6d6d"
BOT_ID = "c4617d9d-f7d4-4410-a555-e23635ce2ceb"
GROUP_ID = "49743066"

conn = sqlite3.connect('sessions.db')
cur = conn.cursor()
longpoll = {}

cur.execute('''CREATE TABLE IF NOT EXISTS sessions
               (vkid text, ssid text)''')


def new_longpoll():
	reqparam = {
		'access_token': ACCESS_TOKEN,
		'v': '5.80',
		'group_id': GROUP_ID
	}
	r = requests.get('https://api.vk.com/method/groups.getLongPollServer',
					 params=reqparam)
	print(r.text)
	longpoll.update(r.json().get('response', {}))
	print('LongPoll was obtained')


def perform_answer(m):
	t = (m['from_id'],)
	session = cur.execute('''SELECT * FROM sessions
                             WHERE vkid=?''', t)
	sres = session.fetchone()
	if not sres:
		ssid = uuid.uuid4()
		p = (m['from_id'], str(ssid.int))
		cur.execute('INSERT INTO sessions VALUES (?, ?)', p)
		conn.commit()
	else:
		ssid = sres[1]
	reqparam = {
		'q': m['text'],
		'sessionId': ssid
	}
	r = requests.get('https://console.dialogflow.com/api-client/demo/embedded/%s/demoQuery' % BOT_ID,
					 params=reqparam)
	res = r.json()['result']['fulfillment']['speech']
	return res


def send(to, text, reqMessage ):
	reqparam = {
		'access_token': ACCESS_TOKEN,
		'v': '5.80',
		'user_id': to,
		'message': text
	}
	r = requests.get('https://api.vk.com/method/messages.send', params=reqparam)
	if r.status_code >= 400:
		reqparam["message"] = f"Что-то пошло не так при обмене сообщениями между ВК и нашей базой ответов :(\n" \
							  f"Ошибка: {r.reason}"
		requests.get('https://api.vk.com/method/messages.send', params=reqparam)
		open("errorLog.txt", "w").write(f"{reqMessage}\n==> {r.reason}")
	return 'response' in r.json()


def startListening():
	while True:
		reqparam = {
			'act': 'a_check',
			'key': longpoll['key'],
			'ts': longpoll['ts'],
			'wait': '25'
		}
		r = requests.get(longpoll['server'], params=reqparam)
		res = r.json()
		if 'failed' in res:
			if res['failed'] == 1:
				longpoll['ts'] = res['ts']
				continue
			elif res['failed'] == 2 or res['failed'] == 3:
				new_longpoll()
				continue
		longpoll['ts'] = res['ts']
		for upd in res['updates']:
			if upd['type'] == 'message_new':
				msg = upd['object']
				if msg['from_id'] == '-%s' % BOT_ID:
					continue
				text_res = perform_answer(msg)
				print('Request: %s' % msg["text"])
				send(msg['from_id'], text_res, msg["text"])
				print('Response: %s' % text_res)
				print('==========')


if __name__ == '__main__':
	while True:
		try:
			new_longpoll()
			startListening()
		except Exception as e:
			open("exceptionLog.txt", 'w').write(str(e.args))
