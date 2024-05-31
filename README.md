# API

## Auth

### Basic (TEMP)

If a valid username/password combo is found in the Accounts table, we create a JWT token withn the GUID of the account and return it to the user in the response body.

```
[POST] /api/v1/login

REQUEST BODY:
{
    "username": "the_username",
    "password": "the_password"
}
```

### Sign in with Apple / Google

TODO: ...

## Accounts

### My account

```
[GET] /api/v1/account
```

## Tournaments

A given account can only have one active tournament at a time.

### Create tournament

```
[POST] /api/v1/tournaments

REQUEST BODY:
{
    "title": "Tietoevry Ping Pong",
    "reset_interval": 0
}
```

### Search for tournament

Search for tournaments matching the query parameter. Returns a list of tournaments.

```
[POST] /api/v1/tournaments/search/{query}
```

### Join tournament

```
[PUT] /api/v1/tournaments/join/{tournament_id}

REQUEST BODY:
{
    "code": "the_code"
}
```

### Get tournament

```
[GET] /api/v1/tournaments

RESPONSE BODY:

{
    "id": "the_guid",
    "title": "the_title",
    "scoreboard": [
        {
            "id": "the_player_id",
            "username": "the_player_username",
            "score": 0,
            "matches_played": 12,
            "matches_won": 10
        },
        ...
    ]
}
```

### Leave tournament

```
[PUT] /api/v1/tournaments/leave
```

### Register match

```
[POST] /api/v1/tournaments/match/{opponent_id}
```

## Models

### Account table

* id (GUID): primary key
* tournament_id (GIUD): foreign key, nullable
* email: valid email (will come from SSO-provider)
* username: the username
* score: integer, default = 1600, ELO-based score
* matches_won: integer, default = 0
* matches_played: integer, default = 0
* created_at: unix timestamp

### Tournament table

* id (GUID): primary key
* admin_id (GUID): foreign key
* title: the tournament title
* reset_interval: integer (0 = monthly, 1 = weekly, 2 = yearly, 3 = never)
* code: string, 6 alphanumeric randomly generated characters (A-Z, a-z, 0-9)

### Match table

* id (GUID): primary key
* tournament_id (GUID): foreign key
* winner_id (GUID): the id of the winner
* winner_delta_score: integer, how many points the winner got
* loser_id (GUID): the id of the loser
* loser_delta_score: integer, how many points the loser lost
* date: unix timestamp of when the match was registered

## Example users

### User 1

{
    "id": "43c9f799-55c1-45e6-ab25-1df1f55cd9cb",
    "email": "user@test1.com",
    "username": "user1"
}

The "sub" claim contains the user ID.

eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI0M2M5Zjc5OS01NWMxLTQ1ZTYtYWIyNS0xZGYxZjU1Y2Q5Y2IiLCJpYXQiOjE1MTYyMzkwMjJ9.nsJkcfkjt_9wOvzJd5ZMEOHGIyqrZ8tShQZIVbUxExg 

### User 2

{
    "id": "8f2902ab-a646-4fac-a9e3-e33d2283f5d8",
    "email": "user@test2.com",
    "username": "user2"
}

The "sub" claim contains the user ID.

eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI4ZjI5MDJhYi1hNjQ2LTRmYWMtYTllMy1lMzNkMjI4M2Y1ZDgiLCJpYXQiOjE1MTYyMzkwMjJ9.HhA7T5nAfQCIgJKhkgPBM_U8SwP48JRhdps0675wwxY

