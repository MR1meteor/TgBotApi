create table if not exists ssh_servers
(
    "Id"           serial
        primary key,
    "CredentialId" integer
        constraint "credentialId_fk"
            references credentials
            on delete set null,
    "Ip"           varchar,
    "Port"         integer,
    "Username"     varchar,
    "Password"     varchar
);