
CREATE TABLE ims_admin (
    id int IDENTITY(1,1) PRIMARY KEY,
    username varchar(20) NOT NULL,
    password text
);

CREATE TABLE ims_categories (
    id int IDENTITY(1,1) PRIMARY KEY,
    category varchar(50) NOT NULL
);

CREATE TABLE ims_products (
    id int IDENTITY(1,1) PRIMARY KEY,
    name varchar(100) NOT NULL,
    category_id int NOT NULL,
    quantity int DEFAULT '0',
    last_modified varchar(40) NOT NULL
);

CREATE TABLE ims_inc_exp (
    id int IDENTITY(1,1) PRIMARY KEY,
    type varchar(10) NOT NULL,
    title varchar(100),
    val float(2) DEFAULT '0.00',
    date_added DATETIME default GETUTCDATE()
);

INSERT INTO ims_admin (username,password) VALUES('admin','pass');
