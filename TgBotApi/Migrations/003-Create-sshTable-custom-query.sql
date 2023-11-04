create table if not exists ssh_custom_queries
(
    "Id"            serial
        primary key,
    "CredentialId" integer
        constraint "credentialId_fk"
            references credentials
            on delete set null,
    "Query"         varchar,
    "QueryName"     varchar
);