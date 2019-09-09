CREATE TABLE Airlines
(  
  Name character varying(80),
  TwoDigitCode character varying(2),
  ThreeDigitCode character varying(3),
  Country character varying(50),
  CONSTRAINT airlines_pkey PRIMARY KEY (TwoDigitCode)
);
