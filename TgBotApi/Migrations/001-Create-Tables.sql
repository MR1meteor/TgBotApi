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
    "Url"         varchar,
    "Name"        varchar
);

create table if not exists queries
(
    "Id"           serial
        constraint queries_pk
            primary key,
    "CredentialsId" integer
        constraint "queries_credentials_Id_fk"
            references credentials,
    "Sql"          varchar,
    "Name"         varchar
);

create table if not exists "query_parameters"
(
    "Id"        serial
        constraint "QueryParameters_pk"
            primary key,
    "QueryId"   integer
        constraint "QueryParameters_queries_Id_fk"
            references queries,
    "Parameter" varchar
);