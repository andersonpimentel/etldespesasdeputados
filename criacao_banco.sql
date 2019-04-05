-- Database: despesasv2db

-- DROP DATABASE "despesasv2db";

CREATE DATABASE "despesasv2db"
    WITH 
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'Portuguese_Brazil.1252'
    LC_CTYPE = 'Portuguese_Brazil.1252'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;

-- User: pentaho
-- DROP USER pentaho;

CREATE USER pentaho WITH
  LOGIN
  SUPERUSER
  INHERIT
  CREATEDB
  CREATEROLE
  NOREPLICATION;

-- Table: public.dim_data

-- DROP TABLE public.dim_data;

CREATE TABLE public.dim_data
(
    cod_data integer,
    dia_mes integer,
    dia_semana integer,
    nome_dia_semana character varying(100) COLLATE pg_catalog."default",
    mes integer,
    nome_mes character varying(100) COLLATE pg_catalog."default",
    ano integer,
    txt_data character varying(100) COLLATE pg_catalog."default",
    vl_data date
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.dim_data
    OWNER to pentaho;

-- Table: public.dim_deputado

-- DROP TABLE public.dim_deputado;

CREATE TABLE public.dim_deputado
(
    cod_deputado integer NOT NULL,
    nome_deputado character varying(200) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT dim_deputado_pkey PRIMARY KEY (cod_deputado)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.dim_deputado
    OWNER to postgres;

-- Table: public.dim_partido

-- DROP TABLE public.dim_partido;

CREATE TABLE public.dim_partido
(
    cod_partido integer NOT NULL,
    sg_partido character varying(20) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT dim_partido_pkey PRIMARY KEY (cod_partido)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.dim_partido
    OWNER to postgres;

-- Table: public.dim_tipo

-- DROP TABLE public.dim_tipo;

CREATE TABLE public.dim_tipo
(
    cod_tipo integer NOT NULL,
    desc_tipo character varying(80) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT dim_tipo_pkey PRIMARY KEY (cod_tipo)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.dim_tipo
    OWNER to postgres;

-- Table: public.dim_uf

-- DROP TABLE public.dim_uf;

CREATE TABLE public.dim_uf
(
    cod_uf integer NOT NULL,
    sg_uf character(2) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT dim_uf_pkey PRIMARY KEY (cod_uf)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.dim_uf
    OWNER to postgres;

-- Table: public.fato_despesas

-- DROP TABLE public.fato_despesas;

CREATE TABLE public.fato_despesas
(
    cod_tipo integer,
    cod_deputado integer,
    cod_partido integer,
    cod_uf integer,
    dt_despesa date,
    vl_despesa money
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.fato_despesas
    OWNER to postgres;
