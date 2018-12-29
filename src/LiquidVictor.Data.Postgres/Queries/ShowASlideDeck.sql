SELECT s.title "slide title", s.layout, sds.sortorder "slide order", ci.contenttype, sci.sortorder "content item order", length(ci.encodedcontent) "content length"
FROM slidedeck sd
inner join slidedeckslides sds
on sds.slidedeckid = sd.id
inner join slides s
on s.id = sds.slideid
inner join slidecontentitems sci
on sci.slideid = s.id
inner join contentitems ci
on ci.id = sci.contentitemid
where sd.id = [slidedeckid]
order by sds.sortorder, sci.sortorder
