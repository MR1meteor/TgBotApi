create table if not exists credentials
(
    "Name"     varchar,
    "UserId"   bigint,
    "Host"     varchar,
    "Port"     varchar,
    "Database" varchar,
    "Username" varchar,
    "Password" varchar,
    "Id"       serial
        primary key
);

create table if not exists Users
(
    "Id"       serial
        primary key,
    "UserId"   bigint
);