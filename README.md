# API

## Auth

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

BODY:
{
    "title": "Tietoevry Ping Pong",
    "reset_interval": 0
}
```

### Join tournament

```
[PUT] /api/v1/tournaments/join
```

### Get tournament

```
[GET] /api/v1/tournaments
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

### Match table

* id (GUID): primary key
* winner_id (GUID): the id of the winner
* winner_delta_score: integer, how many points the winner got
* loser_id (GUID): the id of the loser
* loser_delta_score: integer, how many points the loser lost
* date: unix timestamp of when the match was registered
