#!/bin/bash
database="postgres"

psql -d $database -c "\copy airports FROM 'data/airports.csv' DELIMITER ',' CSV HEADER;"
psql -d $database -c "\copy airlines FROM 'data/airlines.csv' DELIMITER ',' CSV HEADER;"
psql -d $database -c "\copy routes FROM 'data/routes.csv' DELIMITER ',' CSV HEADER;"