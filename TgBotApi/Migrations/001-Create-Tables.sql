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

create table if not exists links
(
    "Id"           serial
        primary key,
    "CredentialId" integer
        constraint "credentialId_fk"
            references credentials
            on delete set null,
    "Url"         varchar
);