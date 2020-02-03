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

##############################################
# 1. TOP GENRES.
##############################################
topGenres <- minableview[,c("Genre","PlaysNumber")]

topGenres <- group_by(topGenres, Genre)
topGenresPlays <- summarise(topGenres, totalPlays = sum(PlaysNumber))

# Get percentage of total.
# From https://stackoverflow.com/a/27135129
topGenresPlays <- topGenresPlays %>% mutate(percent = totalPlays/sum(totalPlays))
topGenresPlays <- topGenresPlays[order(-topGenresPlays$percent),]

# Transform to factor to keep the plot ordered.
# FROM: https://stackoverflow.com/a/20041311
topGenresPlays$Genre <- factor(topGenresPlays$Genre, levels = topGenresPlays$Genre)

png("topGenres-by-popularity.png", height=750, width=1000)

ggplot(topGenresPlays, aes(x= Genre, y=percent)) + geom_bar(stat="identity", fill="steelblue") +
    theme_minimal(base_size=15) +
    ggtitle("Genres by popularity") + theme(plot.title = element_text(hjust = 0.5), axis.text.x = element_text(size=10))

dev.off()

##############################################
# 2. GENRES BY AGE.
##############################################
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

mycolors <- colorRampPalette(brewer.pal(8, "Accent"))(15)

# Plot genres by age
png("topGenres-by-age.png", height=750, width=1000)

ggplot(aggregatedGenre_by_age, aes(x= AgeInterval, y=percent, fill=Genre)) + geom_col() +
    scale_fill_manual(values=mycolors) +
    theme_bw(base_size=15) +
    ggtitle("Genres by Age") + theme(plot.title = element_text(hjust = 0.5))

dev.off()