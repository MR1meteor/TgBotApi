create table if not exists Dumps (
    id serial primary key,
    sql text not null,
    eventDate timestamp default now(),
    credentialsId int not null,

    foreign key(credentialsId) references credentials("Id")
);