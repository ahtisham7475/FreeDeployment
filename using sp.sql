CALL `statuscodes`.`SP_CopyLogStats`();
select * from proxylog order by count asc;
insert into statuscodes.proxylog
 (domain,proxyip, count) 
 values ('asd',3,1)
 
 select * from LastCopyDate
 
 
 insert into LastCopyDate values( CURDATE());
 
 update LastCopyDate set LastCopyDatecol=(select max(LastCopyDatecol)) where 1=1;
 delete from LastCopyDate where id>0;
 insert into  LastCopyDate (LastCopyDatecol) select max(createdate) from statuscodes ;
 
 select * from LastCopyDate