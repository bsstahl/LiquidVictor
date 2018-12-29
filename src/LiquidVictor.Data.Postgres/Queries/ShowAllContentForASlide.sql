select s.id "slide.id", s.title "slide.title", sci.id "slidecontentitem.id", sci.sortorder, ci.id "contentitem.id", ci.contenttype, ci.content, length(ci.content)
from slides s
inner join slidecontentitems sci
on sci.slideid = s.id
inner join contentitems ci
on ci.id = sci.contentitemid
where s.id = [slideid]