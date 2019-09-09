CREATE TABLE Airports
(  
  Name character varying(80),
  City character varying(50),
  Country character varying(50),
  Iata3 character varying(3),
  Latitude double precision,
  Longitude double precision,
  CONSTRAINT airports_pkey PRIMARY KEY (Iata3)
);