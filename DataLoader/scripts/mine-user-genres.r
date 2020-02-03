# Load libraries.
.libs <-c("dplyr", "ggplot2", "RColorBrewer")
sapply(.libs, require, character.only=TRUE)

# Load user data.
userPlays <- read.csv('minableview', sep=',', header=T)
summary(userPlays)

# Remove empty cases.
userPlaysClean <- userPlays[complete.cases(userPlays),]

# Assign the users in age intervals.
cutsUserPlays <- cut(userPlaysClean$Age, breaks=seq(0,100,5))
userPlaysClean$AgeInterval <- cutsUserPlays

# Remove users whose age are outside the 0 - 100 years interval.
userPlaysClean <- userPlaysClean[complete.cases(userPlaysClean),]

# Keep only interesting columns -> Minable View.
columns <- c("Gender", "AgeInterval", "PlaysNumber", "Genre")
minableview <- userPlaysClean[,columns]

# Group genres by age interval.
genre_by_age <- group_by(minableview, AgeInterval, Genre)
aggregatedGenre_by_age <- summarise(genre_by_age, totalPlays = sum(PlaysNumber))

# Sort by most played.
aggregatedGenre_by_age <- aggregatedGenre_by_age[order(aggregatedGenre_by_age$AgeInterval, -aggregatedGenre_by_age$totalPlays),]

# Transform from factor to character, to persist the tibble.
aggregatedGenre_by_age$Genre <- as.character(aggregatedGenre_by_age$Genre)
aggregatedGenre_by_age$AgeInterval <- as.character(aggregatedGenre_by_age$AgeInterval)

# Added the percent column.
# From https://stackoverflow.com/a/27135129
aggregatedGenre_by_age <- aggregatedGenre_by_age %>% mutate(percent = totalPlays/sum(totalPlays))

# Keep only the top genre.
# From https://stackoverflow.com/a/14800643
topGenreByAge <- Reduce(rbind, by(aggregatedGenre_by_age, aggregatedGenre_by_age$AgeInterval, head, n=1))

# Write to disk.
write.csv2(topGenreByAge, 'top_genre_by_age.csv')

mycolors <- colorRampPalette(brewer.pal(11, "Spectral"))(15)

png("topGenres-by-age.png", height=750, width=1000)

ggplot(aggregatedGenre_by_age, aes(x= AgeInterval, y=percent, fill=Genre)) + geom_col() +
    scale_fill_manual(values=mycolors) +
    theme_bw(base_size=15) +
    ggtitle("Genres by Age") + theme(plot.title = element_text(hjust = 0.5))

dev.off()